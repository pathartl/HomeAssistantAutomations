using NetDaemonApps.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetDaemonApps.Extensions
{
    public static class LightExtensions
    {
        private static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static async Task TransitionColors(this LightEntity light, Color from, Color to, TimeSpan duration, int fromBrightness = 255, int toBrightness = 255)
        {
            light.TurnOn(new LightTurnOnParameters()
            {
                Brightness = toBrightness,
                RgbColor = new int[] { to.Red, to.Green, to.Blue },
                Transition = (int)duration.TotalSeconds
            });
        }

        public static async Task TransitionColors(this LightEntity light, Color from, Color to, int steps, TimeSpan duration)
        {
            var timePerStep = duration.TotalMilliseconds / steps;
            var colors = Color.InterpolateColors(from, to, steps);

            foreach (var color in colors)
            {
                light.TurnOn(new LightTurnOnParameters()
                {
                    Brightness = 255,
                    RgbColor = new int[] { color.Red, color.Green, color.Blue }
                });

                await Task.Delay(TimeSpan.FromMilliseconds(timePerStep));
            }
        }

        public static async Task TransitionBrightness(this LightEntity light, int to, TimeSpan duration)
        {
            light.TurnOn(new LightTurnOnParameters()
            {
                Brightness = to,
                Transition = (int)duration.TotalSeconds
            });
        }

        public static async Task TransitionBrightness(this LightEntity light, int to, int steps, TimeSpan duration)
        {
            await TransitionBrightness(light, Convert.ToInt32(light.Attributes?.Brightness), to, steps, duration);
        }

        public static async Task TransitionBrightness(this LightEntity light, int from, int to, int steps, TimeSpan duration)
        {
            var timePerStep = duration.TotalMilliseconds / steps;
            var brightnessPerStep = (to - from) / (double)steps;
            
            for (var i = 1; i <= steps; i++)
            {
                var nextBrightness = from + (brightnessPerStep * i);

                if (nextBrightness != 0)
                {
                    light.TurnOn(new LightTurnOnParameters()
                    {
                        Brightness = (int)Math.Round(nextBrightness)
                    });
                }
                else
                {
                    light.TurnOff();
                }

                await Task.Delay(TimeSpan.FromMilliseconds(timePerStep));
            }
        }
    }
}
