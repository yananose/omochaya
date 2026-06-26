// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StoryTaskMonitor.cs" company="Omochaya">
//   Copyright (c) 2026 Omochaya. All rights reserved.
//   Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// <summary>
//   Provides a custom Unity EditorWindow interface to track, filter, sort, and inspect
//   the live status and execution order of automated and manual tasks during runtime.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
#if (FOR_DEBUG || UNITY_EDITOR) && !STORY_FAST

// 〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜
// これ以降は間接的に使用されます。利用者が直接使用することは想定していません
// 〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜
namespace Omochaya.HiddenStory
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEditor.UIElements;
    using UnityEngine;
    using UnityEngine.UIElements;

    class TaskMonitorForDebug : EditorWindow
    {
        /// <summary>Opens or focuses the custom Omochaya Task Monitor diagnostic editor window.</summary>
        [MenuItem("Window/Omochaya/Task Monitor")]
        public static void ShowWindow()
        {
            var window = GetWindow<TaskMonitorForDebug>("Task Monitor");
            window.minSize = new Vector2(400, 300);
        }

        enum SortMode
        {
            Order,
            OwnerName,
            TaskId
        }

        // UI要素
        ListView listView;
        Label infoLabel;
        HelpBox playModeHelpBox;
        VisualElement headerContainer;
        VisualElement footer;
        ToolbarSearchField searchField;
        ToolbarButton btnSortOrder;

        // データ管理用
        readonly List<Story.Task> allTasks = new();
        readonly List<Story.Task> filteredTasks = new();
        
        // ★ ドメインリロード等での揮発を防ぐためシリアライズ
        [SerializeField] string searchString = ""; 
        bool autoRefresh = true;
        double lastRefreshTime;
        
        // ソート設定
        SortMode sortMode = SortMode.Order;
        bool sortAscending = false; // デフォルトは降順（実行位置が新しいもの上にするため）

        void OnEnable()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }

        void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            // Playモード開始時（編集モード退出時）に検索状態をクリーンアップ
            if (state == PlayModeStateChange.ExitingEditMode)
            {
                this.searchString = "";
                if (this.searchField != null)
                {
                    this.searchField.SetValueWithoutNotify("");
                }
                
                this.filteredTasks.Clear();
                this.listView?.RefreshItems();
            }
        }

        void CreateGUI()
        {
            var root = this.rootVisualElement;

            // 1. ツールバーの構築
            var toolbar = new Toolbar();
            
            var btnRefresh = new ToolbarButton(() => RefreshData()) { text = Messages.EditorUI.TaskMonitor_Refresh };
            toolbar.Add(btnRefresh);

            var toggleAuto = new ToolbarToggle { text = Messages.EditorUI.TaskMonitor_AutoRefresh, value = this.autoRefresh };
            toggleAuto.RegisterValueChangedCallback(evt => this.autoRefresh = evt.newValue);
            toolbar.Add(toggleAuto);

            toolbar.Add(new ToolbarSpacer { style = { flexGrow = 1 } });

            // 検索フィールド
            this.searchField = new ToolbarSearchField();
            this.searchField.value = this.searchString; // 生成時に前回の状態を復元（同期ズレ防止）
            this.searchField.RegisterValueChangedCallback(evt => 
            {
                this.searchString = evt.newValue ?? "";
                RefreshData();
            });
            toolbar.Add(this.searchField);

            root.Add(toolbar);

            // 2. プレイモード警告ボックス
            this.playModeHelpBox = new HelpBox(Messages.EditorUI.TaskMonitor_PlayModeOnly, HelpBoxMessageType.Info);
            root.Add(this.playModeHelpBox);

            // 3. ヘッダー（ソート機能を含む）
            this.headerContainer = new VisualElement();
            this.headerContainer.style.flexDirection = FlexDirection.Row;
            this.headerContainer.style.height = 24;
            this.headerContainer.style.borderBottomWidth = 1;
            this.headerContainer.style.borderBottomColor = Color.gray;
            this.headerContainer.style.backgroundColor = new Color(0.25f, 0.25f, 0.25f, 0.3f);
            this.headerContainer.style.alignItems = Align.Center;
            root.Add(this.headerContainer);

            var headerTitle = new Label("Task Signature") { style = { flexGrow = 1, paddingLeft = 6, unityFontStyleAndWeight = FontStyle.Bold } };
            this.headerContainer.Add(headerTitle);

            var sortLabel = new Label("Sort by:") { style = { paddingRight = 4 } };
            this.headerContainer.Add(sortLabel);

            var sortEnum = new EnumField(this.sortMode) { style = { width = 100 } };
            sortEnum.RegisterValueChangedCallback(evt => 
            {
                this.sortMode = (SortMode)evt.newValue;
                RefreshData();
            });
            this.headerContainer.Add(sortEnum);

            this.btnSortOrder = new ToolbarButton(() => 
            {
                this.sortAscending = !this.sortAscending;
                this.btnSortOrder.text = this.sortAscending ? "▲" : "▼";
                RefreshData();
            }) { text = this.sortAscending ? "▲" : "▼", style = { width = 25, unityTextAlign = TextAnchor.MiddleCenter } };
            this.headerContainer.Add(this.btnSortOrder);

            // 4. ListView の初期化と設定
            this.listView = new ListView
            {
                style = { flexGrow = 1 },
                showAlternatingRowBackgrounds = AlternatingRowBackground.All,
                fixedItemHeight = 20,
                itemsSource = this.filteredTasks
            };

            // 行の見た目（セル）を生成
            this.listView.makeItem = () => new Label { style = { paddingLeft = 6, unityTextAlign = TextAnchor.MiddleLeft } };

            // セルにデータをバインド
            this.listView.bindItem = (element, index) =>
            {
                var label = (Label)element;
                if (index >= 0 && index < this.filteredTasks.Count)
                {
                    label.text = this.filteredTasks[index].ToString();
                }
            };

            // 右クリック（コンテキストメニュー）
            this.listView.RegisterCallback<ContextClickEvent>(evt =>
            {
                int index = this.listView.selectedIndex;
                if (index >= 0 && index < this.filteredTasks.Count)
                {
                    var task = this.filteredTasks[index];
                    var menu = new GenericMenu();
                    menu.AddItem(new GUIContent(Messages.EditorUI.TaskMonitor_MenuPingOwner), false, () =>
                    {
                        Component owner = null;
                        DevForEditor.TaskMonitorAPI.ExtractOwner(ref owner, task);
                        if (owner != null) EditorGUIUtility.PingObject(owner);
                    });
                    menu.AddItem(new GUIContent(Messages.EditorUI.TaskMonitor_MenuForceFree), false, () =>
                    {
                        task.Free();
                        RefreshData();
                    });
                    menu.ShowAsContext();
                }
            });

            root.Add(this.listView);

            // 5. フッター（ステータス情報ラベル）
            this.footer = new VisualElement();
            this.footer.style.height = 24;
            this.footer.style.borderTopWidth = 1;
            this.footer.style.borderTopColor = Color.gray;
            this.footer.style.paddingLeft = 6;
            this.footer.style.justifyContent = Justify.Center;
            this.footer.style.backgroundColor = new Color(0.25f, 0.25f, 0.25f, 0.3f);
            root.Add(this.footer);

            this.infoLabel = new Label { style = { unityFontStyleAndWeight = FontStyle.Bold } };
            this.footer.Add(this.infoLabel);

            // 初回描画
            RefreshData();
        }

        void Update()
        {
            // UI生成前の実行をガード
            if (this.playModeHelpBox == null || this.listView == null) return;

            var isPlaying = Application.isPlaying;
            
            this.playModeHelpBox.style.display = isPlaying ? DisplayStyle.None : DisplayStyle.Flex;
            // this.headerContainer.style.display = isPlaying ? DisplayStyle.Flex : DisplayStyle.None;
            // this.listView.style.display = isPlaying ? DisplayStyle.Flex : DisplayStyle.None;
            // this.footer.style.display = isPlaying ? DisplayStyle.Flex : DisplayStyle.None;

            if (!isPlaying || EditorApplication.isPaused) return;

            // 0.5秒に1回の自動更新
            if (this.autoRefresh && EditorApplication.timeSinceStartup - this.lastRefreshTime > 0.5f)
            {
                RefreshData();
                this.lastRefreshTime = EditorApplication.timeSinceStartup;
            }
        }

        /// <summary>
        /// データの構築とListViewへの反映
        /// </summary>
        void RefreshData()
        {
            if (!Application.isPlaying || this.infoLabel == null) return;

            // UIと内部変数の同期ズレを確実に防ぐため、毎回UIから値を取得する
            if (this.searchField != null)
            {
                this.searchString = this.searchField.value ?? "";
            }

            var autoCount = 0;
            var manualCount = 0;
            var lateCount = 0;
            var fixedCount = 0;
            DevForEditor.TaskMonitorAPI.FetchAutoCount(ref autoCount);
            DevForEditor.TaskMonitorAPI.FetchManualCount(ref manualCount);
            DevForEditor.TaskMonitorAPI.FetchLateCount(ref lateCount);
            DevForEditor.TaskMonitorAPI.FetchFixedCount(ref fixedCount);
            DevForEditor.TaskMonitorAPI.GetTaskList(this.allTasks);

            var waitCount = this.allTasks.Count - (autoCount + manualCount + lateCount + fixedCount);

            // フッター用文字列の生成
            string sAuto = string.Format(Messages.EditorUI.TaskMonitor_StatAuto, autoCount);
            string sManual = string.Format(Messages.EditorUI.TaskMonitor_StatManual, manualCount);
            string sLate = string.Format(Messages.EditorUI.TaskMonitor_StatLate, lateCount);
            string sFixed = string.Format(Messages.EditorUI.TaskMonitor_StatFixed, fixedCount);
            string sWait = string.Format(Messages.EditorUI.TaskMonitor_StatWait, waitCount);
            string sTotal = string.Format(Messages.EditorUI.TaskMonitor_StatTotal, this.allTasks.Count);

            this.infoLabel.text = $"{sAuto}    |    {sManual}    |    {sLate}    |    {sFixed}    |    {sWait}    |    {sTotal}";

            this.filteredTasks.Clear();
            bool hasSearch = !string.IsNullOrEmpty(this.searchString);

            foreach (var t in this.allTasks)
            {
                if (!hasSearch || t.ToString().IndexOf(this.searchString, System.StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    this.filteredTasks.Add(t);
                }
            }

            // ゼロアロケーションでのインプレース・ソート
            this.filteredTasks.Sort((a, b) =>
            {
                var result = 0;
                var offsetA = 0L;
                var offsetB = 0L;
                Component ownerA = null;
                Component ownerB = null;
                switch (this.sortMode)
                {
                    case SortMode.Order:
                        DevForEditor.TaskMonitorAPI.GetOrder(ref offsetA, a);
                        DevForEditor.TaskMonitorAPI.GetOrder(ref offsetB, b);
                        result = offsetA.CompareTo(offsetB);
                        break;
                    case SortMode.OwnerName:
                        DevForEditor.TaskMonitorAPI.GetOrder(ref offsetA, a);
                        DevForEditor.TaskMonitorAPI.GetOrder(ref offsetB, b);
                        DevForEditor.TaskMonitorAPI.ExtractOwner(ref ownerA, a);
                        DevForEditor.TaskMonitorAPI.ExtractOwner(ref ownerB, b);
                        string ma = ownerA?.name ?? string.Empty;
                        string mb = ownerB?.name ?? string.Empty;
                        result = string.CompareOrdinal(ma, mb);
                        // Owner名が同じならOffsetでサブソート
                        if (result == 0) { result = offsetA.CompareTo(offsetB); }
                        break;
                    case SortMode.TaskId:
                        result = a.Id.Index.CompareTo(b.Id.Index);
                        break;
                }
                return this.sortAscending ? result : -result;
            });

            this.listView.RefreshItems();
        }
    }
}
#endif
