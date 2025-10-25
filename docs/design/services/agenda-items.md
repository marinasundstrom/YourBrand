# Agenda item orchestration

This document summarises how agenda items drive the meeting procedure and how actions are captured in code.

## Agenda items

`AgendaItem` is the core entity that models a point on a meeting agenda. It tracks:

- State transitions (`Pending`, `Active`, `Completed`, `Postponed`, `Skipped`, `Canceled`).
- Procedural phases (`Default`, `Discussion`, `Voting`, `Ended`).
- Optional requirements for discussion or voting through `DiscussionActions` and `VoteActions`.

State and phase changes are driven through the domain methods on the entity. For example, `Activate` moves an item from `Pending` or `Postponed` into the `Active` state, while `StartDiscussion` and `StartVoting` set the phase to `Discussion` or `Voting` and initialise the backing `Discussion` or `Voting` objects. `EndVoting`, `Complete`, `Skip`, `Postpone`, and `Cancel` complete the procedural flow and enforce the invariants associated with mandatory agenda items.

Each domain method is effectively an *agenda item action*. The meeting procedure layer can surface these actions to the chairperson or secretary depending on the current state/phase. For example:

- Activate item
- Start or end discussion
- Start, end, or redo voting
- Start or end an election
- Complete, postpone, skip, or cancel the item

By routing the UI or workflow engine through these methods, we centralise validation logic and avoid duplicating the decision rules elsewhere in the system.

## Agenda item types

`AgendaItemType` defines reusable templates (call to order, approval of minutes, motions, elections, etc.). These types allow the application to derive the default set of actions that make sense for a particular item. For instance, motion and election types both require voting, while a simple announcement might only allow completion. The enumeration-like members declared on `AgendaItemType` enable the application to preconfigure agenda builders and action menus.

### Action descriptors

Each type ships with a catalogue of `AgendaItemActionDescriptor`s that describe the domain method to call, any preconditions (state/phase, supporting data such as candidates to register, etc.), and the minute template to emit when the action completes. The descriptor is the basis for the command surface presented to the chairperson or secretary. Examples include:

- `ActivateAgendaItem`
- `StartDiscussion`
- `RecordDiscussionSummary`
- `OpenMotionVoting`
- `CloseVotingAndPublishResults`
- `CompleteAgendaItem`

Descriptors are declarative and can be queried at any time to build the list of *potential* actions for an agenda item.

### Meeting-level coordination

Some actions span multiple aggregates—for example, beginning a recess pauses the meeting and all agenda items. Meeting-level actions follow the same descriptor pattern through `MeetingActionDescriptor`, which may delegate to one or more agenda item actions under the hood. `MeetingAction`s are surfaced alongside agenda item actions so that the workflow engine can present a complete procedure to the facilitator.

## Recording outcomes in minutes

Actions culminate in the automatic creation of minute entries. The `Minutes` aggregate contains helpers for adding `MinutesItem` records with a reference to the originating `AgendaItemType` and optional `AgendaItem` identifiers. This is where the predefined logic for each agenda item type lives: when an action completes, its handler can call `Minutes.AddItem` to append a new entry describing the outcome (for example, the result of an election). These minute items are fully editable afterwards, allowing the secretary to refine wording or add supplemental details before approval.

## Action handlers

To keep the agenda workflow cohesive, action handlers should be implemented as thin domain services that orchestrate the relevant `AgendaItem` method and persist the corresponding minute item. A typical handler will:

1. Load the current meeting, agenda, agenda item, and minutes aggregate root.
2. Invoke the agenda item method that performs the state transition.
3. Use the `Minutes` aggregate to record the outcome in a `MinutesItem`, pre-populated according to the agenda item type.
4. Allow the secretary to edit the generated minute item prior to publication or approval.

This approach keeps the meeting flow deterministic while still leaving room for manual adjustments to the official minutes.

### Handler contract

Handlers expose a consistent contract so the UI and automation layers can drive the workflow without bespoke branching:

```csharp
Task<AgendaItemActionResult> HandleAsync(AgendaItemActionCommand command, CancellationToken ct)
```

Where `AgendaItemActionCommand` identifies the meeting, agenda item, action descriptor, and any payload (for example, ballot options or attachments). `AgendaItemActionResult` contains:

- The updated agenda item projection (state, phase, and metadata).
- A reference to the created or updated minute item.
- The collection of *next possible actions* derived from the descriptors that are now valid.

Returning the next actions immediately after completion means the client can refresh the UI without recomputing availability, providing a clear step-by-step procedure.

### Determining next actions

The next actions come from an `AgendaItemActionGraph` associated with the agenda item type. The graph links descriptors to their follow-ups based on domain rules. For instance, completing `StartDiscussion` unlocks `RecordDiscussionSummary` and `OpenMotionVoting`; completing `OpenMotionVoting` unlocks `CloseVotingAndPublishResults` and possibly `RedoVote`. When the handler resolves `AgendaItemActionResult`, it walks the graph and filters by the updated agenda item state/phase to determine the valid next steps. The same graph powers a `PreviewActions` query that surfaces the upcoming procedure before any interaction, helping facilitators anticipate the meeting flow.

### Meeting timeline

Meeting actions (recess, adjourn, reopen) also participate in a timeline graph. Completing a meeting action returns both meeting-level next steps and agenda item actions, so the facilitator always sees the full set of choices in context. Agenda item actions that require the meeting to be `InSession` automatically disappear when the meeting is recessed, ensuring procedural integrity.
