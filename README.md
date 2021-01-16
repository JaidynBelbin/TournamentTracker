# TournamentTracker
Tournament Tracking program in C# based on the Tim Corey tutorial on FCC.

Handles the creation and tracking of multiple elimination-style tournaments or competitions. Requires the participants' first and last names, phone number, and email address (for updating them about the tournament as it progresses and for handing out prizes). People can then be assigned into teams, and as many teams as you wish can be enrolled in the tournament. 

After providing a tournament name and creating prizes (optional), the competition can begin! 

# How It Works:

The teams are randomly matched up in each round, and the winners of each round face each other in the next round. If there are an uneven number of teams in any round, one of the teams is randomly assigned a bye, and they automatically progress to the next round. The last two teams standing play each other in the final round. 

After each round, an email is sent to each team member notifying them of what team they are facing next. Once the tournament is complete, an email is sent to all the players, congratulating the winner and thanking them all for playing. If prizes were up for grabs in the tournament, then the winners are also told how much they receive.

# How to Run It:

In the final version, the user will have the option to save the data to *either* a CSV file or an SQL database. Currently there is a bug preventing the SQL option from working correctly, so currently the CSV file is the only option. In order to save to a CSV file, simply provide the desired file path from the Welcome form, and away you go.

# Coming Soon:

> Fix for the SQL issue

> Fix for finding SMTP client

> Handling format of final email when no prizes in Tournament


