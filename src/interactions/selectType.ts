import { StringSelectMenuInteraction } from 'discord.js';

export async function handleTypeSelection(interaction: StringSelectMenuInteraction) {
  const selectedType = interaction.values[0];

  const today = new Date();
  const options = Array.from({ length: 7 }).map((_, i) => {
    const date = new Date(today);
    date.setDate(today.getDate() + i);
    const label = date.toLocaleDateString('en-US', { weekday: 'short', month: 'short', day: 'numeric' });
    const value = date.toISOString().split('T')[0];
    return { label, value };
  });

  await interaction.update({
    content: `📅 You selected **${selectedType.toUpperCase()}**.\n\n🗓️ Choose a date:`,
    components: [
      {
        type: 1,
        components: [
          {
            type: 3,
            custom_id: `select-date|${selectedType}`,
            placeholder: 'Pick a date',
            options
          }
        ]
      }
    ]
  });
}
