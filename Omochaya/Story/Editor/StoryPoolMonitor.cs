// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StoryPoolMonitor.cs" company="Omochaya">
//   Copyright (c) 2026 Omochaya. All rights reserved.
//   Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// <summary>
//   Provides a custom Unity EditorWindow interface using UI Toolkit to visualize, monitor, and profile real-time
//   memory footprints and allocation capacities of active object pools within the framework.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#if (FOR_DEBUG || UNITY_EDITOR) && !STORY_NO_DEBUG

// 〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜
// これ以降は間接的に使用されます。利用者が直接使用することは想定していません
// 〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜
namespace Omochaya.HiddenStory
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;
    using UnityEditor.UIElements;
    using UnityEngine;
    using UnityEngine.UIElements;

    class PoolMonitorForDebug : EditorWindow
    {
        enum SortColumn { Name, Active, Worst, Free, MemorySize }

        [MenuItem("Window/Omochaya/Pool Monitor")]
        public static void ShowWindow()
        {
            var window = GetWindow<PoolMonitorForDebug>("Pool Monitor");
            window.minSize = new Vector2(500, 300);
        }

        // 状態管理
        readonly List<IPoolMonitorForDebug> displayList = new();
        ListView listView;
        Label infoLabel;
        HelpBox playModeHelpBox;

        SortColumn currentSortColumn = SortColumn.Name;
        bool sortAscending = true;

        public void CreateGUI()
        {
            var root = this.rootVisualElement;

            // 1. ツールバーの作成
            var toolbar = new Toolbar();
            root.Add(toolbar);

            // 2. プレイモード警告ボックス
            this.playModeHelpBox = new HelpBox(Messages.EditorUI.PoolMonitor_PlayModeOnly, HelpBoxMessageType.Info);
            root.Add(this.playModeHelpBox);

            // 3. ヘッダー（ソートボタン付きの行）の作成
            var headerContainer = new VisualElement();
            headerContainer.style.flexDirection = FlexDirection.Row;
            headerContainer.style.height = 24;
            headerContainer.style.flexShrink = 0;
            headerContainer.style.borderBottomWidth = 1;
            headerContainer.style.borderBottomColor = Color.gray;
            headerContainer.style.backgroundColor = new Color(0.25f, 0.25f, 0.25f, 0.3f);
            root.Add(headerContainer);

            var btnActive = CreateHeaderButton(Messages.EditorUI.PoolMonitor_ColActive, SortColumn.Active, 60);
            var btnWorst = CreateHeaderButton(Messages.EditorUI.PoolMonitor_ColWorst, SortColumn.Worst, 60);
            var btnFree = CreateHeaderButton(Messages.EditorUI.PoolMonitor_ColFree, SortColumn.Free, 60);
            var btnMemory = CreateHeaderButton(Messages.EditorUI.PoolMonitor_ColMemory, SortColumn.MemorySize, 100);
            var btnName = CreateHeaderButton(Messages.EditorUI.PoolMonitor_ColName, SortColumn.Name, 0);
            btnName.style.flexGrow = 1;

            headerContainer.Add(btnActive);
            headerContainer.Add(btnWorst);
            headerContainer.Add(btnFree);
            headerContainer.Add(btnMemory);
            headerContainer.Add(btnName);

            // 4. リストビューの初期化
            this.listView = new ListView(this.displayList, 20, MakeItem, BindItem);
            this.listView.style.flexGrow = 1;
            this.listView.showAlternatingRowBackgrounds = AlternatingRowBackground.All;
            root.Add(this.listView);

            // 5. フッター（ステータス情報ラベル）の作成
            var footer = new VisualElement();
            footer.style.height = 24;
            footer.style.flexShrink = 0;
            footer.style.borderTopWidth = 1;
            footer.style.borderTopColor = Color.gray;
            footer.style.paddingLeft = 6;
            footer.style.justifyContent = Justify.Center;
            footer.style.backgroundColor = new Color(0.25f, 0.25f, 0.25f, 0.3f);
            root.Add(footer);

            this.infoLabel = new Label { style = { unityFontStyleAndWeight = FontStyle.Bold } }; // 修正: unityFontStyleAndWeight
            footer.Add(this.infoLabel);

            RefreshDisplay();
        }

        Button CreateHeaderButton(string text, SortColumn column, float width)
        {
            var button = new Button(() => ToggleSort(column)) { text = text };
            button.style.borderTopWidth = 0;
            button.style.borderBottomWidth = 0;
            button.style.borderLeftWidth = 0;
            button.style.borderRightWidth = 1;
            button.style.borderRightColor = Color.gray;
            
            // 修正: 個別の角丸プロパティで 0 を指定
            button.style.borderTopLeftRadius = 0;
            button.style.borderTopRightRadius = 0;
            button.style.borderBottomLeftRadius = 0;
            button.style.borderBottomRightRadius = 0;
            
            button.style.unityTextAlign = TextAnchor.MiddleLeft;
            button.style.backgroundColor = Color.clear;
            if (width > 0)
            {
                button.style.width = width;
            }
            return button;
        }

        VisualElement MakeItem()
        {
            var row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;

            var activeLabel = new Label() { name = "Active", style = { width = 65, paddingLeft = 4, unityTextAlign = TextAnchor.MiddleLeft } };
            var worstLabel = new Label() { name = "Worst", style = { width = 65, paddingLeft = 4, unityTextAlign = TextAnchor.MiddleLeft } };
            var freeLabel = new Label() { name = "Free", style = { width = 65, paddingLeft = 4, unityTextAlign = TextAnchor.MiddleLeft } };
            var memoryLabel = new Label() { name = "Memory", style = { width = 100, paddingLeft = 4, unityTextAlign = TextAnchor.MiddleLeft } };
            var nameLabel = new Label() { name = "Name", style = { flexGrow = 1, paddingLeft = 4, unityTextAlign = TextAnchor.MiddleLeft } };

            row.Add(activeLabel);
            row.Add(worstLabel);
            row.Add(freeLabel);
            row.Add(memoryLabel);
            row.Add(nameLabel);

            return row;
        }

        void BindItem(VisualElement element, int index)
        {
            if (index < 0 || index >= this.displayList.Count) return;

            var monitor = this.displayList[index];
            if (monitor == null) return;

            element.Q<Label>("Active").text = monitor.ActiveCount.ToString();
            element.Q<Label>("Worst").text = monitor.WorstCount.ToString();
            element.Q<Label>("Free").text = monitor.FreeCount.ToString();
            element.Q<Label>("Memory").text = DevForEditor.FormatMemorySize(monitor.TotalBytes);
            element.Q<Label>("Name").text = monitor.PoolName;
        }

        void Update()
        {
            // UI生成前の実行をガード
            if (this.playModeHelpBox == null || this.listView == null) return;

            var isPlaying = Application.isPlaying;

            // プレイモード状態に応じてUIの表示・非表示を切り替え
            this.playModeHelpBox.style.display = isPlaying ? DisplayStyle.None : DisplayStyle.Flex;
            // this.listView.style.display = isPlaying ? DisplayStyle.Flex : DisplayStyle.None;

            // プレイモードでない、または一時停止中の場合はここで処理を打ち切る
            if (!isPlaying || EditorApplication.isPaused)
            {
                // // プレイモードを抜けた瞬間のみリストをクリアする
                // if (!isPlaying && this.displayList.Count > 0)
                // {
                //     this.displayList.Clear();
                //     this.listView.RefreshItems();
                // }
                return;
            }

            // --- これ以降はプレイ中かつ動いている時のみ実行される ---

            foreach (var monitor in IPoolMonitorForDebug.Monitors)
            {
                if (monitor != null)
                {
                    monitor.WorstCount = Mathf.Max(monitor.WorstCount, monitor.ActiveCount);
                }
            }

            this.displayList.Clear();
            this.displayList.AddRange(IPoolMonitorForDebug.Monitors.Where(m => m != null));

            ApplySort();
            RefreshFooter();
        }

        void ToggleSort(SortColumn column)
        {
            if (this.currentSortColumn == column)
            {
                this.sortAscending = !this.sortAscending;
            }
            else
            {
                this.currentSortColumn = column;
                this.sortAscending = (column == SortColumn.Name);
            }

            RefreshDisplay();
        }

        void ApplySort()
        {
            this.displayList.Sort((a, b) =>
            {
                int result = 0;
                switch (this.currentSortColumn)
                {
                    case SortColumn.Name:
                        result = string.CompareOrdinal(a.PoolName, b.PoolName);
                        break;
                    case SortColumn.Active:
                        result = a.ActiveCount.CompareTo(b.ActiveCount);
                        break;
                    case SortColumn.Worst:
                        result = a.WorstCount.CompareTo(b.WorstCount);
                        break;
                    case SortColumn.Free:
                        result = a.FreeCount.CompareTo(b.FreeCount);
                        break;
                    case SortColumn.MemorySize:
                        result = a.TotalBytes.CompareTo(b.TotalBytes);
                        break;
                }
                return this.sortAscending ? result : -result;
            });

            this.listView?.RefreshItems();
        }

        void RefreshDisplay()
        {
            if (this.rootVisualElement == null) return;

            UpdateButtonText(SortColumn.Active, Messages.EditorUI.PoolMonitor_ColActive);
            UpdateButtonText(SortColumn.Worst, Messages.EditorUI.PoolMonitor_ColWorst);
            UpdateButtonText(SortColumn.Free, Messages.EditorUI.PoolMonitor_ColFree);
            UpdateButtonText(SortColumn.MemorySize, Messages.EditorUI.PoolMonitor_ColMemory);
            UpdateButtonText(SortColumn.Name, Messages.EditorUI.PoolMonitor_ColName);

            ApplySort();
        }

        void UpdateButtonText(SortColumn column, string baseText)
        {
            var btn = this.rootVisualElement.Query<Button>().Where(b => b.text.StartsWith(baseText) || b.text.Contains(baseText)).First();
            if (btn != null)
            {
                if (this.currentSortColumn == column)
                {
                    btn.text = baseText + (this.sortAscending ? " ▲" : " ▼");
                }
                else
                {
                    btn.text = baseText;
                }
            }
        }

        void RefreshFooter()
        {
            if (this.infoLabel == null) return;

            long grandTotalBytes = 0;
            double grandTotalActiveBytes = 0;

            foreach (var monitor in this.displayList)
            {
                grandTotalBytes += monitor.TotalBytes;
                int totalCount = monitor.ActiveCount + monitor.FreeCount;
                if (totalCount > 0)
                {
                    grandTotalActiveBytes += monitor.TotalBytes * (monitor.ActiveCount / (double)totalCount);
                }
            }

            string countStr = string.Format(Messages.EditorUI.PoolMonitor_StatCount, IPoolMonitorForDebug.Monitors.Count);
            
            double usageRate = grandTotalBytes > 0 ? grandTotalActiveBytes / grandTotalBytes : 0.0;
            string usageStr = string.Format(Messages.EditorUI.PoolMonitor_StatUsage, usageRate);
            
            string memoryStr = string.Format(Messages.EditorUI.PoolMonitor_StatMemory, DevForEditor.FormatMemorySize((int)Mathf.Min(grandTotalBytes, int.MaxValue)));

            this.infoLabel.text = $"{countStr}    |    {usageStr}    |    {memoryStr}";
        }
    }
}
#endif
