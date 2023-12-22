#!/usr/bin/env python3
import matplotlib.pyplot as plt
import numpy as np

def annotate_interval(ax, p0, p1, text, dxtext=None, dytext=None, linewidth=1, fontsize=12, ha='center', color='black'):
    # stolen from https://stackoverflow.com/a/38683607 with some modification
    ax.annotate('', xy=p0, xytext=p1, xycoords='data', textcoords='data',
            arrowprops={'arrowstyle': '|-|', 'linewidth':linewidth, 'shrinkA':0, 'shrinkB':0, 'color':color})
    ax.annotate('', xy=p0, xytext=p1, xycoords='data', textcoords='data',
            arrowprops={'arrowstyle': '<->', 'linewidth':linewidth, 'shrinkA':0, 'shrinkB':0, 'color':color})

    xcenter = (p1[0]+p0[0])/2
    ycenter = (p1[1]+p0[1])/2
    if dxtext is None:
        dxtext = 0
    if dytext is None:
        dytext = ( ax.get_ylim()[1] - ax.get_ylim()[0] ) / 25

    ax.annotate(text, xy=(xcenter + dxtext, ycenter + dytext), ha=ha, va='center', fontsize=fontsize, color=color)

def draw_help_image():
    PULSE_STRENGTH = 1.0
    RUMBLE_DURATION = 20.0
    PULSE_FREQUENCY = 1.0
    PULSE_DURATION = 4.0
    PULSE_TRAIN_PERIOD = 8.0

    PADDING_S = 1.0
    PADDING_T = 2.0


    fig, ax = plt.subplots(figsize=(15,5), facecolor='#d8d8d8')

    # time
    t = np.arange(0.0 - PADDING_T, RUMBLE_DURATION + PADDING_T, PULSE_FREQUENCY / 100.0)

    # mask to give some padding before and after the rumble
    mask = np.where((t >= 0) & (t <= RUMBLE_DURATION), 1, 0)

    pulse = np.sin(2*np.pi*PULSE_FREQUENCY*t)
    pulse_train = np.mod(np.floor(t/PULSE_DURATION)+1, 2)

    s = mask * pulse * pulse_train

    line, = ax.plot(t, s, lw=2)

    ax.set_ylim(-PULSE_STRENGTH-PADDING_S, PULSE_STRENGTH+PADDING_S)
    ax.set_xlim(t[0], t[-1])

    annotate_interval(ax, (0, -PULSE_STRENGTH*1.05), (PULSE_DURATION, -PULSE_STRENGTH*1.05), 'Pulse duration', dytext=-0.2)
    annotate_interval(ax, (0, PULSE_STRENGTH*1.05), (PULSE_TRAIN_PERIOD, PULSE_STRENGTH*1.05), "Pulse train period")
    annotate_interval(ax, (0, PULSE_STRENGTH*1.5), (RUMBLE_DURATION, PULSE_STRENGTH*1.5), "Rumble duration")

    end_of_second_pulse = PULSE_TRAIN_PERIOD + PULSE_DURATION
    annotate_interval(ax, (end_of_second_pulse + 0.3, -PULSE_STRENGTH), (end_of_second_pulse + 0.3, PULSE_STRENGTH), "Strength", dxtext=0.1, ha='left')

    first_trough_x = np.ceil(PULSE_TRAIN_PERIOD / PULSE_FREQUENCY) + 3/4
    second_trough_x = first_trough_x + 1/PULSE_FREQUENCY
    annotate_interval(ax, (first_trough_x, -PULSE_STRENGTH*1.15), (second_trough_x, -PULSE_STRENGTH*1.15), r'$(\text{Pulse frequency})^{-1}$', dytext=-0.25)

    # disable axes because the exact values are not important in this context
    ax.set_axis_off()

    

    return fig

#with plt.style.context("fivethirtyeight"):
draw_help_image().savefig(f"pulse-train.webp", format="webp", bbox_inches="tight")