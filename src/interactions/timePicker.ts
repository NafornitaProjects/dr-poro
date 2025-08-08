import {
  StringSelectMenuInteraction,
  ActionRowBuilder,
  StringSelectMenuOptionBuilder,
  StringSelectMenuBuilder,
} from 'discord.js';

export async function handleTimeSelection(interaction: StringSelectMenuInteraction) {
  const [prefix, date, type] = interaction.customId.split('|'); // select-time|2025-08-11|scrim
  const hour = interaction.values[0];

  const minuteOptions = ['00', '15', '30', '45'].map(time => {
    const newTime = `${hour.substring(0, 2)}:${time}` // 19:00 => [19:00, 19:15, 19:30, 19:45]
    return new StringSelectMenuOptionBuilder().setLabel(newTime).setValue(newTime);
  });

  const timeMenu = new StringSelectMenuBuilder()
    .setCustomId(`select-half-hour|${date}|${hour}|${type}`)
    .setPlaceholder(`Pick a time`)
    .addOptions(minuteOptions);

  const row = new ActionRowBuilder<StringSelectMenuBuilder>().addComponents(timeMenu);

  await interaction.update({
    content: `✅ Time: **${hour}**\n\n⏰ Let's be more specific:`,
    components: [row],
  });
}