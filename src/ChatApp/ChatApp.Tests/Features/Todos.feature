Feature: Todos
As a User, I want to keep track of what to do.

@mytag
Scenario: Create a Todo
	Given that you define a task to do
	When you post it
	Then then an item should have been created