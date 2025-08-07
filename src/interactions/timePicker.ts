import {
  StringSelectMenuInteraction,
  ModalBuilder,
  TextInputBuilder,
  TextInputStyle,
  ActionRowBuilder,
} from 'discord.js';

export async function handleTimeSelection(interaction: StringSelectMenuInteraction) {
  const [prefix, date, type] = interaction.customId.split('|'); // select-time|2025-08-11|scrim
  const time = interaction.values[0];

  const modal = new ModalBuilder()
    .setCustomId(`team-input|${date}|${time}|${type}`)
    .setTitle('Who are we up against?')
    .addComponents(
      new ActionRowBuilder<TextInputBuilder>().addComponents(
        new TextInputBuilder()
          .setCustomId('opponent')
          .setLabel('Enter opponent name')
          .setStyle(TextInputStyle.Short)
          .setRequired(true)
      )
    );

  await interaction.showModal(modal);
}
