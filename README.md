# Change-And-Error-Management-System


I am not sure if i am alone in this, but i noticed that throughout the life cycle of my projects the agreed functionalities, issues and changes aren't centrally logged electronically for future reference purpose to the benefit of my clients and me. I wasn't sure if such a platform existed so i build one, because well... I could :). The application serves as an online changes/errors (issue) logging system where changes/errors can be logged and the status of logged issue/errors can be flagged through "submitted", to "being processed", to "processed", to "processing confirmed" (final flag). There are three classes of users:

1. User: (https://logger.solarinolakunle.com/User/login) these are the clients that usually initiate (submit) the changes/errors under projects (projects are created and managed by the admin user class) and flag them as "processing confirmed". They can also create and manage user roles under which they can profile other users.

2. Admin: (https://logger.solarinolakunle.com/Admin/login) these are the service providers by whom the change/errors are to be reviewed. They can initiate (submit) changes/errors under projects but can only flag changes/errors as "being processed" or "processed". They can also:

a) create and manage admin roles under which they can profile other admin in their company.  

b) create and manage organizations (clients) under which they can profile the main client user who will be the first user that can manage everything that has to do with that organization (client).

c) create and manage projects under which changes/issues will be logged.

3. Super Admin: (https://logger.solarinolakunle.com/SuperAdmin/login) these are viewers

of most of the information that exists on the application. For the purpose of monitoring some of the activity on the application. Because I didn't feel there might be need for more than one user under this user class, i just have one with a default email "first@first.com" and password "first" but i have changed the password on my site (this information is only useful when implementing the application outside my site. They have access to the following:

a) View all admin

b) View all Users

c) View All organizations

d) View All Service providers (all admin must be profiled under a service provider)

e) View All projects

Issue/errors logging or flagging sends emails to all users in the affected user classes (user, admin).

There is an "sql.txt" file that creates the database tables and populates some of the tables as needed.

Please check out the application at the urls i have shared.

User: https://logger.solarinolakunle.com/User/login

Admin: https://logger.solarinolakunle.com/Admin/login 

I'll appreciate suggestions and feedback.
 
