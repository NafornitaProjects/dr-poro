import { ModalSubmitInteraction, ButtonBuilder, ActionRowBuilder, ButtonStyle } from 'discord.js';

export async function handleTeamInput(interaction: ModalSubmitInteraction) {
  const opponent = interaction.fields.getTextInputValue('opponent');

  const [prefix, date, time, type] = interaction.customId.split('|');

  if (!date || !time || !type || !opponent) {
    await interaction.reply({
      content: '❌ Missing required event details. Please try again.',
      ephemeral: true,
    });
    return;
  }

  const confirm = new ButtonBuilder()
    .setCustomId(`confirm-post|${date}|${time}|${type}|${opponent}`)
    .setLabel('✅ Confirm')
    .setStyle(ButtonStyle.Success);

  const row = new ActionRowBuilder<ButtonBuilder>().addComponents(confirm);

  await interaction.reply({
    content: `📋 **Event Preview**\n🆚 Opponent: **${opponent}**\n📅 Date: ${date}\n🕒 Time: ${time}\n🏷️ Type: ${type.toUpperCase()}\n\nReady to create the event?`,
    components: [row],
    ephemeral: true,
  });
}
