using System;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace Blastic.Maui.ControlExtensions;

public class FadeTriggerAction : TriggerAction<VisualElement>
{
	public double FadeTo { get; set; }
	public TimeSpan Duration { get; set; }

	protected override void Invoke(VisualElement sender)
	{
		double currentOpacity = sender.Opacity;
		double difference = FadeTo - currentOpacity;

		sender.Animate(
			"FadeTriggerAction",
			new Animation(x =>
			{
				sender.Opacity = currentOpacity + x * difference;
			}),
			length: (uint)Duration.TotalMilliseconds,
			easing: Easing.Linear);
	}
}