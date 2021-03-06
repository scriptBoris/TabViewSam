﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TabViewSam.Utils;
using Xamarin.Forms;

namespace TabViewSam
{
    [ContentProperty("TabsContent")]
    [Xamarin.Forms.Internals.Preserve(AllMembers = true)]
    public sealed class TabView : Grid
    {
        private readonly Grid headerGrid;
        private Tab oldSelectedTab;
        private Tab selectedTab;
        internal Tab SelectedTab { 
            get => selectedTab;
            private set {
                selectedTab = value;
                OnSelectedTab(selectedTab);
            }
        }

        public TabView()
        {
            // Header & Content
            RowSpacing = 0;
            RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });

            headerGrid = new Grid();
            headerGrid.HorizontalOptions = LayoutOptions.FillAndExpand;
            headerGrid.ColumnSpacing = 0;
            SetRow(headerGrid, 0);
        }

        private void InitTabs()
        {
            // Prepare
            Children.Clear();
            Children.Add(headerGrid);
            headerGrid.Children.Clear();
            headerGrid.ColumnDefinitions.Clear();
            SelectedTab = null;

            if (Tabs == null)
                return;

            // Start init
            int i = 0;
            int actualId = 0;
            foreach (var item in Tabs)
            {
                item.SetHost(this);
                item.TabId = i;
                headerGrid.Children.Add(item.TabHeadCell);

                // Add content
                if (item.Content != null)
                {
                    SetRow(item.Content, 1);
                    Children.Add(item.Content);
                }

                if (item.IsVisible)
                {
                    headerGrid.ColumnDefinitions.Add(new ColumnDefinition
                    {
                        Width = new GridLength(1, GridUnitType.Star)
                    });
                    SetColumn(item.TabHeadCell, actualId);
                    actualId++;

                    if (SelectedTab == null)
                    {
                        SelectedTab = item;
                        item.Content.IsVisible = true;
                    }
                    else
                    {
                        item.Content.IsVisible = false;
                    }
                }

                i++;
            }
        }

        private void UnselectTab(Tab tab)
        {
            if (tab == null)
                return;

            tab.TabHeadCell.BackgroundColor = ColorSelector.Set(tab.BackgroundColor, TabsBackgroundColor);
            tab.Footer.BackgroundColor = Color.Default;
        }

        private void OnSelectedTab(Tab selectTab)
        {
            var o = oldSelectedTab;
            var n = selectTab;

            if (n == null)
            {
                UnselectTab(o);
                oldSelectedTab = null;
                return;
            }

            if (n == o)
                return;

            UnselectTab(o);

            var setBg = ColorSelector.Set(n.SelectedColor, SelectedColor);
            if (!setBg.IsDefault)
                n.TabHeadCell.BackgroundColor = ColorSelector.Set(n.SelectedColor, SelectedColor);
            else
                n.TabHeadCell.BackgroundColor = TabsBackgroundColor;

            n.Footer.BackgroundColor = ColorSelector.Set(n.SelectedFooterColor, SelectedFooterColor);

            oldSelectedTab = n;
        }

        private static void OnChangedTitleFontSize(BindableObject b, object o, object n)
        {
            var self = b as TabView;
            if (self.Tabs == null || self.Tabs.Count == 0)
                return;

            foreach (var item in self.Tabs)
            {
                if (item.TitleFontSize != null)
                    item.LabelTitle.FontSize = (double)n;
            }
        }

        private static void OnChangedTitleColor(BindableObject b, object o, object n)
        {
            var self = b as TabView;
            if (self.Tabs == null || self.Tabs.Count == 0)
                return;

            foreach (var item in self.Tabs)
            {
                if (!item.TitleColor.IsDefault)
                    item.LabelTitle.TextColor = (Color)n;
            }
        }

        private static void OnChangedSelectedColor(BindableObject b, object o, object n)
        {
            var self = b as TabView;
            if (self.Tabs == null || self.Tabs.Count == 0 || self.SelectedTab == null)
                return;

            if (self.SelectedTab.SelectedColor.IsDefault)
                self.SelectedTab.TabHeadCell.BackgroundColor = (Color)n;
        }

        private static void OnChangedSelectedFooterColor(BindableObject b, object o, object n)
        {
            var self = b as TabView;
            if (self.Tabs == null || self.Tabs.Count == 0 || self.SelectedTab == null)
                return;

            if (self.SelectedTab.SelectedFooterColor.IsDefault || self.SelectedTab.SelectedFooterColor == Color.Transparent)
                self.SelectedTab.Footer.BackgroundColor = (Color)n;
        }

        private static void OnChangedTabsBackgroundColor(BindableObject b, object o, object n)
        {
            var self = b as TabView;
            if (self.Tabs == null || self.Tabs.Count == 0)
                return;

            foreach (var item in self.Tabs)
            {
                if (!item.BackgroundColor.IsDefault && item != self.SelectedTab)
                    item.TabHeadCell.BackgroundColor = (Color)n;
            }
        }

        private static void OnChangedIsShowTabs(BindableObject b, object o, object n)
        {
            var self = b as TabView;
            bool value = (bool)n;
            self.headerGrid.IsVisible = value;
        }

        private static void OnChangedSelectedTabIndex(BindableObject b, object o, object n)
        {
            var self = b as TabView;
            int index = (int)n;

            if (self.Tabs.Count == 0)
                return;

            if (index < 0)
                index = 0;

            if (index > self.Tabs.Count - 1)
                index = self.Tabs.Count - 1;

            self.OpenTab(index);
            //self.SelectedTab = self.Tabs[index];
        }

        internal void UpdateTabsVisibility(Tab tabChanged, bool isVisible)
        {
            if (IsShowTabs)
            {
                tabChanged.TabHeadCell.IsVisible = isVisible;

                foreach (var item in Tabs)
                {
                    var definition = headerGrid.ColumnDefinitions[item.TabId];

                    if (item.TabHeadCell.IsVisible)
                    {
                        definition.Width = GridLength.Star;
                    }
                    else
                    {
                        definition.Width = new GridLength(0, GridUnitType.Absolute);
                    }
                }
            }

            if (SelectedTab == tabChanged && !isVisible)
            {
                SelectedTab.Content.IsVisible = false;
                foreach (var item in Tabs)
                {
                    if (item.IsVisible && SelectedTab != item)
                    {
                        SelectedTab = item;
                        SelectedTab.Content.IsVisible = true;
                        break;
                    }
                }

                if (SelectedTab == tabChanged)
                    SelectedTab = null;
            }
            // Set visible if no content
            else if (SelectedTab == null && isVisible)
            {
                SelectedTab = tabChanged;
                SelectedTab.Content.IsVisible = true;
            }
        }

        internal void OpenTab(int index)
        {
            var tab = Tabs[index];

            if (SelectedTab != null)
            {
                //if (lastTab == tab)
                //    return;
                SelectedTab.IsSelected = false;
                SelectedTab.Content.IsVisible = false;
            }
            else
            {
                foreach (var item in Tabs)
                {
                    item.IsSelected = false;
                    item.Content.IsVisible = false;
                }
            }

            SelectedTab = tab;
            SelectedTab.Content.IsVisible = true;
            SelectedTab.IsSelected = true;
        }

        protected override void OnParentSet()
        {
            InitTabs();
            base.OnParentSet();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            if (Tabs?.Count > 0)
            {
                foreach (var item in Tabs)
                    item.BindingContext = BindingContext;
            }
        }

        // Title font size
        public static readonly BindableProperty TitleFontSizeProperty =
            BindableProperty.Create(nameof(TitleFontSize), typeof(double), typeof(TabView), 16.0,
            propertyChanged: OnChangedTitleFontSize);
        public double TitleFontSize
        {
            get { return (double)GetValue(TitleFontSizeProperty); }
            set { SetValue(TitleFontSizeProperty, value); }
        }

        // Title color
        public static readonly BindableProperty TitleColorProperty =
            BindableProperty.Create(nameof(TitleColor), typeof(Color), typeof(TabView), Color.Black,
            propertyChanged: OnChangedTitleColor);
        public Color TitleColor
        {
            get { return (Color)GetValue(TitleColorProperty); }
            set { SetValue(TitleColorProperty, value); }
        }

        // Selected color
        public static readonly BindableProperty SelectedColorProperty =
            BindableProperty.Create(nameof(SelectedColor), typeof(Color), typeof(TabView),
                propertyChanged: OnChangedSelectedColor);
        public Color SelectedColor
        {
            get { return (Color)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }

        // Selected footer color
        public static readonly BindableProperty SelectedFooterColorProperty =
            BindableProperty.Create(nameof(SelectedFooterColor), typeof(Color), typeof(TabView),
                propertyChanged: OnChangedSelectedFooterColor);
        public Color SelectedFooterColor
        {
            get { return (Color)GetValue(SelectedFooterColorProperty); }
            set { SetValue(SelectedFooterColorProperty, value); }
        }

        // Tabs background color
        public static readonly BindableProperty TabsBackgroundColorProperty =
            BindableProperty.Create(nameof(TabsBackgroundColor), typeof(Color), typeof(TabView), Color.FromHex("#80d6ff"),
                propertyChanged: OnChangedTabsBackgroundColor);
        public Color TabsBackgroundColor
        {
            get { return (Color)GetValue(TabsBackgroundColorProperty); }
            set { SetValue(TabsBackgroundColorProperty, value); }
        }

        // Is show tabs
        public static readonly BindableProperty IsShowTabsProperty =
            BindableProperty.Create(nameof(IsShowTabs), typeof(bool), typeof(TabView), true,
                propertyChanged: OnChangedIsShowTabs);
        public bool IsShowTabs
        {
            get { return (bool)GetValue(IsShowTabsProperty); }
            set { SetValue(IsShowTabsProperty, value); }
        }

        // Selected tab index
        public static readonly BindableProperty SelectedTabIndexProperty =
            BindableProperty.Create(nameof(SelectedTabIndex), typeof(int), typeof(TabView), 0,
                propertyChanged: OnChangedSelectedTabIndex);
        public int SelectedTabIndex
        {
            get { return (int)GetValue(SelectedTabIndexProperty); }
            set { SetValue(SelectedTabIndexProperty, value); }
        }

        /// <summary>
        /// Tabs
        /// </summary>
        public static readonly BindableProperty TabsProperty =
            BindableProperty.Create(nameof(Tabs), typeof(List<Tab>), typeof(TabView),
                propertyChanged: (b, o, n) =>
                {
                    (b as TabView).InitTabs();
                },
                defaultValueCreator: b =>
                {
                    return new List<Tab>();
                });
        public List<Tab> Tabs
        {
            get { return (List<Tab>)GetValue(TabsProperty); }
            set { SetValue(TabsProperty, value); }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public List<Tab> TabsContent => Tabs;
    }
}
