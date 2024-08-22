Feature: Chat
As a User, I want to keep track of what to do.

@mytag
Scenario: User is joining a channel
    Given that a user is not already a participant in a channel
	When they join the channel
	Then they become a participant in the channel
    And they have subscribed for notifications

@mytag
Scenario: Participant is leaving channel
    Given that a user is already a participant in a channel
    When the participant leaves the channel
    Then they are no longer participants
    And the user are no longer subscribed for notifications