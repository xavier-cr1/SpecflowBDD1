﻿Feature: MessagesService

#For sending a private message to user, always verify the user exists
Background: 
     Given The forum receives a request for obtaining the user list
     And The user with the username 'xaviercr1' is in registered users list

@Type:API
#Send a private message to users himself, and check if this message exists (obtain this new private message)
Scenario: Obtain a new private message sent to a user
    Given The username 'xaviercr1' sends a private message with the following properties
        | Message                          |
        | Sending to myself a test message |
    Then The status code for sending a private message in the forum is '200'
    When The username 'xaviercr1' and password 'Xavier1234.' sends a request to obtain its private message list
    And The status code for obtaining the private message list is '200'
    Then The message list has the new message 'Sending to myself a test message'