import dotenv from 'dotenv';
dotenv.config();

import { Client, GatewayIntentBits, Events } from 'discord.js';
import { handleScheduleCommand } from './commands/schedule.js';
import { handleDateSelection } from './interactions/datePicker.js';
import { handleTimeSelection } from './interactions/timePicker.js';
import { handleTeamInput } from './interactions/teamInput.js';
import { handleConfirmation } from './interactions/confirmPost.js';
import { handleTypeSelection } from './interactions/selectType.js';
import { handleHalfHourSelection } from './interactions/selectHalfHour.js';

const client = new Client({
  intents: [GatewayIntentBits.Guilds],
});

client.once(Events.ClientReady, () => {
  console.log(`✅ Logged in as ${client.user?.tag}`);
});

client.on(Events.InteractionCreate, async (interaction) => {
  if (interaction.isChatInputCommand() && interaction.commandName === 'schedule') {
    await handleScheduleCommand(interaction);
  }

  if (interaction.isStringSelectMenu()) {
    if (interaction.customId === 'select-type') await handleTypeSelection(interaction);
    if (interaction.customId.startsWith('select-date|')) await handleDateSelection(interaction);
    if (interaction.customId.startsWith('select-time|')) await handleTimeSelection(interaction);
    if (interaction.customId.startsWith('select-half-hour|')) await handleHalfHourSelection(interaction);
  }

  if (interaction.isModalSubmit() && interaction.customId.startsWith('team-input|')) {
    await handleTeamInput(interaction);
  }

  if (interaction.isButton() && interaction.customId.startsWith('confirm-post|')) {
    await handleConfirmation(interaction);
  }
});

client.login(process.env.DISCORD_TOKEN);
