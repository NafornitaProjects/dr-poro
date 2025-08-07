import { REST, Routes, SlashCommandBuilder } from 'discord.js';
import dotenv from 'dotenv';
dotenv.config();

const commands = [
  new SlashCommandBuilder()
    .setName('schedule')
    .setDescription('Schedule a tournament or scrim match')
    .toJSON()
];

const rest = new REST({ version: '10' }).setToken(process.env.DISCORD_TOKEN!);

(async () => {
  try {
    console.log('🔁 Registering slash commands...');
    await rest.put(
      Routes.applicationGuildCommands(process.env.CLIENT_ID!, process.env.GUILD_ID!),
      { body: commands }
    );
    console.log('✅ Slash command registered.');
  } catch (error) {
    console.error('❌ Failed to register:', error);
  }
})();
