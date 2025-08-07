import {
  ChatInputCommandInteraction,
  ActionRowBuilder,
  StringSelectMenuBuilder,
  StringSelectMenuOptionBuilder
} from 'discord.js';

export async function handleScheduleCommand(interaction: ChatInputCommandInteraction) {
  const options = ['scrim', 'matchday'].map(type =>
    new StringSelectMenuOptionBuilder().setLabel(type.toUpperCase()).setValue(type)
  );

  const typeMenu = new StringSelectMenuBuilder()
    .setCustomId('select-type')
    .setPlaceholder('Scrim or Match Day?')
    .addOptions(options);

  const row = new ActionRowBuilder<StringSelectMenuBuilder>().addComponents(typeMenu);

  await interaction.deferReply({ ephemeral: true });

  await interaction.editReply({
    content: '🏷️ What type of match is this?\n\n',
    components: [row],
  });
}
