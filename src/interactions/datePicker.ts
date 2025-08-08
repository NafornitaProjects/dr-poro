import {
  StringSelectMenuInteraction,
  ActionRowBuilder,
  StringSelectMenuBuilder,
  StringSelectMenuOptionBuilder,
} from 'discord.js';

export async function handleDateSelection(interaction: StringSelectMenuInteraction) {
  try {
    const selectedDate = interaction.values[0];

    const [, type] = interaction.customId.split('|');

    const timeOptions = ['18:00', '19:00', '20:00', '21:00'].map(time =>
      new StringSelectMenuOptionBuilder().setLabel(time).setValue(time)
    );

    const timeMenu = new StringSelectMenuBuilder()
      .setCustomId(`select-time|${selectedDate}|${type}`)
      .setPlaceholder('Pick a time')
      .addOptions(timeOptions);

    const row = new ActionRowBuilder<StringSelectMenuBuilder>().addComponents(timeMenu);

    await interaction.update({
      content: `✅ Date: **${selectedDate}**\n\n⏰ Now choose a time:\n(Don't worry, you'll be able to specify minutes after)`,
      components: [row],
    });
  } catch (error) {
    console.error('❌ Error in handleDateSelection:', error);
    await interaction.followUp({
      content: '⚠️ Something went wrong while selecting the date.',
      ephemeral: true,
    });
  }
}
