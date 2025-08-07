import {
  ButtonInteraction,
  GuildScheduledEventEntityType,
  GuildScheduledEventPrivacyLevel,
} from 'discord.js';

export async function handleConfirmation(interaction: ButtonInteraction) {
  const [_, date, time, type, opponent] = interaction.customId.split('|');

  if (!date || !time || !type || !opponent) {
    await interaction.reply({
      content: '❌ Missing data. Please restart the form.',
      ephemeral: true,
    });
    return;
  }

  const startTime = new Date(`${date}T${time}:00`);
  const endTime = new Date(startTime.getTime() + 3 * 60 * 60 * 1000); // 3 hours

  const event = await interaction.guild?.scheduledEvents.create({
    name: `${type === 'match' ? 'Match Day' : 'Scrim'} vs ${opponent}`,
    scheduledStartTime: startTime,
    scheduledEndTime: endTime,
    privacyLevel: GuildScheduledEventPrivacyLevel.GuildOnly,
    entityType: GuildScheduledEventEntityType.External,
    description: `League of Legends ${type === 'match' ? 'Match Day' : 'Scrim'} vs ${opponent}`,
    entityMetadata: {
      location: 'Discord Voice or TBD',
    },
  });

  await interaction.reply({
    content: `✅ Event created: **${event?.name}**\n🕒 ${startTime.toLocaleString()} → ${endTime.toLocaleTimeString()}`,
    ephemeral: true,
  });
}
