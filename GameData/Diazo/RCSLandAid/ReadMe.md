Kill﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿ your horizontal velocity before landing, or hover over a selected point on the ground.

Found on﻿ the CKAN﻿ as Horizontal Landing﻿ Aid

Want to control your vertical velocity?

Automated Vertical Velocity Control

﻿ iconBlue.jpg Button when showing Blue. Button also shows Red and Gray (or transparent to show window color technically).

The button has 3 colors:

Gray: Mod is off and will not activate.

Left-Click to go to Zero Mode (Blue)

Right-Click to go to Location Hover mode (Red)

Blue: Mod is activated and will cancel your horizontal velocity as fast as it can

Left-Click to turn mod off (Gray)

Right-Click to go to Location Hover mode (Red)

Red: Mod is on and in Location Hover mode.

Left-Click to turn mod off (Gray)

Right﻿-Click to go to Zero mode (Blue)

Yellow Border: When the mod is actively controlling the vessel, the icon will have a yellow border around it.

Engage Height: Regardless of which mode the mod is in, it will not activate unless you vessel is within 500 meters of the ground. As the mod zero's out your velocity relative to a point on the surface, zeroing out too high wastes fuel as your vessels' sideways velocity is higher the then sideways velocity of the ground under it. This limit is adjustable by right-clicking the LA button and entering the height you wish to engage at.

SAS: SAS must be enabled to use the mod. In fact, the mod works by controlling the SAS direction. This means that while this mod is enabled, the vessel﻿ control keys (WASDQE) are not effective as the mod will override their inputs as soon as you release them.

RCS: While vessel tip is the primary method of control, if the RCS system is on and there are RCS blocks on the vessel, the mod will use the RCS to help out. Particularly on lower gravity worlds, this speeds up the rate at which the vessel responds as even the maximum default tip of 20Ã‚Â° only provides a sideways acceleration of about 15% the force of gravity currently pulling downwards on the vessel. Note that the RCS system does not affect if the mod is engaged or not, it is optional. (Unlike the SAS system which must be engaged for the mod to control the vessel.)

To Use:

Zero Mode (Blue): In this mode, the mod will simply cancel your horizontal velocity. Once canceled it will keep you hovering over whichever point on the ground you are over. This is intended for landing a skycrane with a horizontal velocity of zero so there is no chance of tipping over from having too much sideways momentum.

Location Hover (Red): When you enter this mod, a red arrow will attach itself to your mouse. Left-click on the ground to set a target location, the red arrow will now attach itself to this point. This mod will then take your vessel and attempt to hover over the red arrow. Note that it is likely your vessel will overshoot on its first approach I am still tweaking the move-to-point math to get it exactly as I want. I will further tweak this based on people's reports.

Note: For the purposes of location, the part you have selected as the "Control From Here" part is what the mod will keep above the target. If you have a docking port on the side of your vessel, you can "Control from Here" and rotate your vessel around that point for a precision landing.

Note2: Right clicking to enter Location Hover mode also opens the settings screen.

Settings: (Saved on a per vessel basis)

Engage Height: The mod will only take control at or below this height above terrain. Note that the default altimeter at the top of the screen displays height above sea level. Defaults to 500m.

Max Tip: How far off vertical the mod will allow the vessel to go. Defaults to 20° Larger vessels with a slower torque response may wish to lower this.

Speed﻿%: This is the speed with, or the aggressiveness, the mod uses. A lower number will result in a slower approach to target and a slower max speed while traveling. In percent, defaults to 100%. Really large vessels, or odd configurations (such as engines above the center of mass) may benefit from changing this number but player preference will vary.

Use RCS only?: You can have this mod use the RCS only to move your vessel if you set the max tip to 0 and engage the RCS thrusters. Note that this will be highly vessel dependent, it can be painfully slow on larger vessels as effective RCS thrust is so low.

