import sys
import json
import numpy as np
import matplotlib.pyplot as plt

from datetime import datetime

def parse_time(t_str):
    """Convert a time like '9:00 PM' to a float hour (e.g., 21.0)."""
    time = datetime.strptime(t_str, "%I:%M %p")
    return time.hour + time.minute / 60


def format_time_label(hour):
    """Format a float hour as 'h:MM AM/PM'."""
    hour_rounded = int(hour)
    minute = int((hour - hour_rounded) * 60)
    return datetime.strptime(f"{hour_rounded}:{minute:02d}", "%H:%M").strftime("%I:%M %p").lstrip("0")


def generate_availability_image(output_path, availability_json):
    """Generate a team availability heatmap visualization."""
    raw_data = json.loads(availability_json)
    days = ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"]

    hours = np.arange(18, 24, 0.5)
    heatmap = np.zeros((len(days), len(hours)))

    for schedule in raw_data.values():
        for day, time_range in schedule.items():
            start_str, end_str = time_range.split(" - ")
            start = parse_time(start_str)
            end = parse_time(end_str)

            # Handle crossing midnight
            if end < start:
                end += 24

            day_idx = days.index(day)

            # Mark all hours within availability window
            for i, h in enumerate(hours):
                if start <= h < end or (end > 24 and h < end - 24):
                    heatmap[day_idx, i] += 1

    rotated_heatmap = heatmap.T
    max_people = np.max(rotated_heatmap) if np.max(rotated_heatmap) > 0 else 1


    fig, ax = plt.subplots(figsize=(8, 10))
    im = ax.imshow(rotated_heatmap, aspect="auto", origin="upper", cmap="Greens")

    cbar = plt.colorbar(im, ax=ax, label="People Available")
    cbar.set_ticks(range(int(max_people) + 1))

    ax.set_xticks(np.arange(len(days)))
    ax.set_xticklabels(days, rotation=45, ha='right')
    ax.set_yticks(np.arange(len(hours)))
    ax.set_yticklabels([format_time_label(h) for h in hours])
    ax.set_xlabel("Day of Week")
    ax.set_ylabel("Time of Day")
    ax.set_title("Team Availability Heatmap")

    plt.tight_layout()

    if output_path:
        plt.savefig(output_path, dpi=150, bbox_inches='tight')
    else:
        plt.show()


if __name__ == '__main__':
    output_path = sys.argv[1]
    availability_json = sys.argv[2]

    generate_availability_image('./availability_heatmap.png', availability_json)
