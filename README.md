QuestTracker is a C# console application that combines a simple task management system with elements inspired by role-playing games. 
The idea behind the project is to make everyday task tracking feel more engaging, 
as if the user were managing a list of quests in an adventure game. 
Each quest can represent a real-life goal, assignment, or responsibility, turning productivity into progress.

The application starts with a registration and login system that includes support for two-factor authentication (2FA). 
When a new user registers, they create a hero profile with a username and password. During login, the program verifies the credentials and a 2FA will be send a one-time code  via SMS. 
The code must be entered correctly before the user can access their account. This adds a layer of security and realism to the experience.

Once logged in, the user is greeted with a clean, menu-based interface that makes navigation straightforward. 
They can add new quests by entering a title and description, view a list of ongoing quests, mark quests as completed, or remove them entirely.
The system is designed to be simple but expandable, allowing future features like deadlines, priorities, 24 hours due deadline User will be notified.

Behind the scenes, the codebase follows a structured design. The Quest class defines the core data model, holding information such as title, description, and status. 
The Hero class stores basic user data, including credentials and contact details for 2FA. 
The QuestManagement class handles the logic for adding, updating, and listing quests, while the MenuHelper class manages the console interaction and user prompts. 


QuestTracker is built on .NET 8 and runs entirely in the terminal. It is lightweight, easy to compile, and portable across operating systems that support the .NET runtime. 
The long-term goal of QuestTracker is to blend functionality with creativity. 
It’s both a coding exercise and a concept experiment in how gamification can make even basic console tools feel more meaningful. 


QuestTracker is ongoing project created by Bozhidar Ivanov as part of exploration of C#, .NET development, and applied programming concepts. 
It demonstrates principles such as object-oriented design, separation of concerns, and secure authentication.
