import { ActionRowBuilder, ChatInputCommandInteraction, ModalBuilder, StringSelectMenuInteraction, TextInputBuilder, TextInputStyle, time } from "discord.js";
import { type } from "os";

export async function handleHalfHourSelection(interaction: StringSelectMenuInteraction) {
    const [prefix, date, time, type] = interaction.customId.split("|");
    const newTime = interaction.values[0];

    const modal = new ModalBuilder()
        .setCustomId(`team-input|${date}|${newTime}|${type}`)
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