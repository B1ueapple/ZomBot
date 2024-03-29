# ZomBot
Discord bot for running an HVZ Discord server.

### Setup ([Website API](https://github.com/redxdev/hvzsite))
1. Run the program. A console window should appear and immediately close, there should now be a "Resources" folder in the directory of your .exe.
2. Open the config.json file inside "Resources" and replace "insert bot token here" with your bot's token surrounded by quotes.
3. Put your hvz website exactly as it is when you visit it including the "http://" in the quotes next to "hvzwebsite".
4. Put your api key (obtained at "{website}/api/v2/auth/apikey") in the quotes next to "apikey"
5. Set "apionline" to true if (and only if) your bot is setup with a website and api key and the website is expected to be up.
6. Run the program again to start the bot.

### Commands
- `/blacklist {user} [true/false]` - Toggles whether or not Zombot will automagically update a specific user. **Requires 'Manage Roles' permission on Discord**
- `/channels {mode} [type]` - Register the current channel as a specific game channel. **Requires 'Manage Channels' permission on Discord**
- `/checklinked` - List all linked and unlinked players. **Requires 'Manage Roles' permission on Discord**
- `/clancolor {r} {g} {b}` - Change the color of your clan role.
- `/endgame` - Update server to reflect the game ending. (Doesn't affect the website) **Requires 'Manage Guild' permission on Discord**
- `/find {name}` - Find a player's discord by their hvz name.
- `/link {name}` - Link your discord to hvz. You must be registered on the website first.
- `/linkbutton` - Send a "Link Account" and "OZ Applicaiton" button in the channel. Clicking the button opens a window to make linking accounts or applying for OZ easier. **Requires 'Manage Roles' permission on Discord**
- `/linkother {user} {name}` - Link someone's discord to hvz. They must be registered on the website first. **Requires 'Manage Roles' permission on Discord**
- `/status [user]` - See a player's hvz related information. Mods also see their zombie ID and human IDs.
- `/unlink [other user]` - Unlink [your] discord from hvz. Does nothing if you aren't currently linked. **Requires 'Manage Roles' permission on Discord to unlink others**
- `/whois {user}` - Check a player's name.

### Passive Functions
- Automagically update a user's role to 'Human', 'Zombie', 'Mod', based on their team on the website.
- Add additional roles for special statuses such as 'Cured'.
- Push missions posted on the website directly to Discord and ping the related teams.
- Allows zombies to compete for the 'MVZ' role which is given to the zombie(s) with the most tags.
- Allows humans to squad together on Discord by creating/joining 'clans' on the website.
- Logs changed/deleted messages in the event that an unsavory individual desecrates the server and tries to cover it up.
- Ending the game will clean up game roles and allow players to provide feedback in an automagically created group of channels and archive game channels. Players are given veteran roles to show their participance in the game.
