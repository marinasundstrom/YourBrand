
# Development methodologies

## Domain-driven design (DDD)

A development approach where developers and stakeholders together model the business domain, aiming at understanding it, to easier develop and maintain a software solution.

## Behavior-driven development (BDD)

The software is documented and designed around behavior and not just requirements.

That means creating behavior specifications that can be automated in order to verify behavior.

## Test-driven development (TDD)

The idea of making verification a part of the development routine by writing code that verifies (is "testing") the behavior, even before the behavior has been implemented.

So not "testing" in the way one might otherwise think.

A common method when employing TDD is Red-Green-Refactor. Meaning that you create a failing verification, or "test", with code stubs. After you have implemented the logic in the stubs the test should pass - gets green (in the Test Explorer UI). And if the test doesn't pass you continue write code and refactor, until it does.

This way you start thinking about what you are building before implementing and then testing behavior outside in.