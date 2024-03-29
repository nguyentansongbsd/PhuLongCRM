﻿using System;
using Android.Views;
using Com.Tomergoldst.Tooltips;
using PhuLongCRM.Droid.Services;
using PhuLongCRM.IServices;
using PhuLongCRM.Models;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using static Com.Tomergoldst.Tooltips.ToolTipsManager;

[assembly: ResolutionGroupName("CrossGeeks")]
[assembly: ExportEffect(typeof(DroidTooltipEffect), nameof(TooltipEffect))]
namespace PhuLongCRM.Droid.Services
{
    public class DroidTooltipEffect : PlatformEffect
    {
        ToolTip toolTipView;
        ToolTipsManager _toolTipsManager;
        ITipListener listener;

        public DroidTooltipEffect()
        {
            listener = new TipListener();
            _toolTipsManager = new ToolTipsManager(listener);
        }

        void OnTap(object sender, EventArgs e)
        {
            var control = Control ?? Container;

            var text = TooltipEffect.GetText(Element);

            if (!string.IsNullOrEmpty(text))
            {
                ToolTip.Builder builder;
                var parentContent = control.RootView;

                var position = TooltipEffect.GetPosition(Element);
                switch (position)
                {
                    case TooltipPosition.Top:
                        builder = new ToolTip.Builder(control.Context, control, parentContent as ViewGroup, text.PadRight(160, ' '), ToolTip.PositionAbove);
                        builder.SetAlign(ToolTip.AlignLeft);
                        break;
                    case TooltipPosition.Left:
                        builder = new ToolTip.Builder(control.Context, control, parentContent as ViewGroup, text.PadRight(160, ' '), ToolTip.PositionLeftTo);
                        builder.SetAlign(ToolTip.AlignLeft);
                        break;
                    case TooltipPosition.Right:
                        builder = new ToolTip.Builder(control.Context, control, parentContent as ViewGroup, text.PadRight(160, ' '), ToolTip.PositionRightTo);
                        builder.SetAlign(ToolTip.AlignLeft);
                        break;
                    case TooltipPosition.BottomRight:
                        builder = new ToolTip.Builder(control.Context, control, parentContent as ViewGroup, text.PadRight(160, ' '), ToolTip.PositionBelow);
                        builder.SetAlign(ToolTip.AlignRight);
                        break;
                    case TooltipPosition.BottomLeft:
                        builder = new ToolTip.Builder(control.Context, control, parentContent as ViewGroup, text.PadRight(160, ' '), ToolTip.PositionBelow);
                        builder.SetAlign(ToolTip.AlignLeft);
                        break;
                    case TooltipPosition.BottomCenter:
                        builder = new ToolTip.Builder(control.Context, control, parentContent as ViewGroup, text.PadRight(160, ' '), ToolTip.PositionBelow);
                        builder.SetAlign(ToolTip.AlignCenter);
                        break;
                    default:
                        builder = new ToolTip.Builder(control.Context, control, parentContent as ViewGroup, text.PadRight(160, ' '), ToolTip.PositionBelow);
                        builder.SetAlign(ToolTip.AlignLeft);
                        break;
                }
                builder.SetBackgroundColor(TooltipEffect.GetBackgroundColor(Element).ToAndroid());
                builder.SetTextColor(TooltipEffect.GetTextColor(Element).ToAndroid());
                if (ListToolTip.ToolTips != null && ListToolTip.ToolTips.Count > 0)
                {
                    foreach (var view in ListToolTip.ToolTips)
                    {
                        if (TooltipEffect.GetText(view) == text)
                            TooltipEffect.SetHasTooltip(view, true);
                        else
                        {
                            TooltipEffect.SetHasTooltip(view, true);
                            TooltipEffect.SetHasTooltip(view, false);
                            TooltipEffect.SetHasTooltip(view, true);
                        }
                    }
                }
                toolTipView = builder.Build();
                _toolTipsManager?.Show(toolTipView);
            }

        }
        protected override void OnAttached()
        {
            var control = Control ?? Container;
            control.Click += OnTap;
        }

        protected override void OnDetached()
        {
            var control = Control ?? Container;
            control.Click -= OnTap;
            _toolTipsManager.FindAndDismiss(control);
        }
        class TipListener : Java.Lang.Object, ITipListener
        {
            public void OnTipDismissed(Android.Views.View p0, int p1, bool p2)
            {

            }
        }
    }
}