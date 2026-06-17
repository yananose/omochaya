// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StoryTask.cs" company="Omochaya">
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

    public class TaskMonitorForDebug : EditorWindow
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
            MasterName,
            TaskId
        }

        // UI要素
        ListView listView;
        Label lblAutoCount;
        Label lblManualCount;
        Label lblWaitingCount;
        Label lblTotalCount;
        HelpBox playModeHelpBox;
        VisualElement dashboard;
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

        // ★ Playモード切り替え時のイベントフック
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
            // ★ Playモード開始時（編集モード退出時）に検索状態をクリーンアップ
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
            // 1. ツールバーの構築
            var toolbar = new Toolbar();
            
            var btnRefresh = new ToolbarButton(() => RefreshData()) { text = Messages.EditorUI.TaskMonitor_Refresh };
            toolbar.Add(btnRefresh);

            var toggleAuto = new ToolbarToggle { text = Messages.EditorUI.TaskMonitor_AutoRefresh, value = this.autoRefresh };
            toggleAuto.RegisterValueChangedCallback(evt => this.autoRefresh = evt.newValue);
            toolbar.Add(toggleAuto);

            toolbar.Add(new ToolbarSpacer { style = { width = 15, flexGrow = 0 } });

            // ソート条件のドロップダウン
            var sortEnum = new EnumField(this.sortMode) { style = { width = 100 } };
            sortEnum.RegisterValueChangedCallback(evt => 
            {
                this.sortMode = (SortMode)evt.newValue;
                RefreshData();
            });
            toolbar.Add(sortEnum);

            // 昇順/降順の切り替えボタン
            this.btnSortOrder = new ToolbarButton(() => 
            {
                this.sortAscending = !this.sortAscending;
                this.btnSortOrder.text = this.sortAscending ? "▲" : "▼";
                RefreshData();
            }) { text = this.sortAscending ? "▲" : "▼", style = { width = 25 } };
            toolbar.Add(this.btnSortOrder);

            toolbar.Add(new ToolbarSpacer { style = { flexGrow = 1 } });

            // 検索フィールド
            this.searchField = new ToolbarSearchField();
            this.searchField.value = this.searchString; // ★ 生成時に前回の状態を復元（同期ズレ防止）
            this.searchField.RegisterValueChangedCallback(evt => 
            {
                this.searchString = evt.newValue ?? "";
                RefreshData();
            });
            toolbar.Add(this.searchField);

            rootVisualElement.Add(toolbar);

            // 2. プレイモード警告ボックス
            this.playModeHelpBox = new HelpBox(Messages.EditorUI.TaskMonitor_PlayModeOnly, HelpBoxMessageType.Info);
            rootVisualElement.Add(this.playModeHelpBox);

            // 3. ダッシュボード（健康状態）
            this.dashboard = new VisualElement { style = { flexDirection = FlexDirection.Row, paddingBottom = 4, paddingTop = 4, paddingLeft = 6, paddingRight = 6 } };
            this.dashboard.style.backgroundColor = new StyleColor(new Color(0.25f, 0.25f, 0.25f, 0.3f));
            
            this.lblAutoCount = new Label();
            this.lblManualCount = new Label { style = { marginLeft = 10 } };
            this.lblWaitingCount = new Label { style = { marginLeft = 10 } };
            this.lblTotalCount = new Label { style = { marginLeft = 10 } };
            
            this.dashboard.Add(this.lblAutoCount);
            this.dashboard.Add(this.lblManualCount);
            this.dashboard.Add(this.lblWaitingCount);
            this.dashboard.Add(this.lblTotalCount);
            rootVisualElement.Add(this.dashboard);

            // 4. ListView の初期化と設定
            this.listView = new ListView
            {
                style = { flexGrow = 1 },
                showAlternatingRowBackgrounds = AlternatingRowBackground.All,
                fixedItemHeight = 20,
                itemsSource = this.filteredTasks
            };

            // 行の見た目（セル）を生成
            this.listView.makeItem = () => new Label { style = { paddingLeft = 10, unityTextAlign = TextAnchor.MiddleLeft } };

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
                    menu.AddItem(new GUIContent(Messages.EditorUI.TaskMonitor_MenuPingMaster), false, () =>
                    {
                        Component master = null;
                        DevForEditor.TaskMonitorAPI.ExtractMaster(ref master, task);
                        if (master != null) EditorGUIUtility.PingObject(master);
                    });
                    menu.AddItem(new GUIContent(Messages.EditorUI.TaskMonitor_MenuForceFree), false, () =>
                    {
                        task.Free();
                        RefreshData();
                    });
                    menu.ShowAsContext();
                }
            });

            rootVisualElement.Add(this.listView);

            // 初回描画
            RefreshData();
        }

        void Update()
        {
            var isPlaying = Application.isPlaying;
            this.playModeHelpBox.style.display = isPlaying ? DisplayStyle.None : DisplayStyle.Flex;
            this.dashboard.style.display = isPlaying ? DisplayStyle.Flex : DisplayStyle.None;
            this.listView.style.display = isPlaying ? DisplayStyle.Flex : DisplayStyle.None;

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
            if (!Application.isPlaying) return;

            // ★ UIと内部変数の同期ズレを確実に防ぐため、毎回UIから値を取得する
            if (this.searchField != null)
            {
                this.searchString = this.searchField.value ?? "";
            }

            var autoCount = 0;
            var manualCount = 0;
            DevForEditor.TaskMonitorAPI.FetchAutoCount(ref autoCount);
            DevForEditor.TaskMonitorAPI.GetManualCount(ref manualCount);
            this.lblAutoCount.text = string.Format(Messages.EditorUI.TaskMonitor_StatAuto, autoCount);
            this.lblManualCount.text = string.Format(Messages.EditorUI.TaskMonitor_StatManual, manualCount);

            DevForEditor.TaskMonitorAPI.GetTaskList(this.allTasks);
            this.lblTotalCount.text = string.Format(Messages.EditorUI.TaskMonitor_StatTotal, this.allTasks.Count);

            var waitCount = this.allTasks.Count - (autoCount + manualCount);
            this.lblWaitingCount.text = string.Format(Messages.EditorUI.TaskMonitor_StatWait, waitCount);

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
                var offsetA = 0;
                var offsetB = 0;
                Component masterA = null;
                Component masterB = null;
                switch (this.sortMode)
                {
                    case SortMode.Order:
                        DevForEditor.TaskMonitorAPI.GetOrder(ref offsetA, a);
                        DevForEditor.TaskMonitorAPI.GetOrder(ref offsetB, b);
                        result = offsetA.CompareTo(offsetB);
                        break;
                    case SortMode.MasterName:
                        DevForEditor.TaskMonitorAPI.GetOrder(ref offsetA, a);
                        DevForEditor.TaskMonitorAPI.GetOrder(ref offsetB, b);
                        DevForEditor.TaskMonitorAPI.ExtractMaster(ref masterA, a);
                        DevForEditor.TaskMonitorAPI.ExtractMaster(ref masterB, b);
                        string ma = masterA?.name ?? string.Empty;
                        string mb = masterB?.name ?? string.Empty;
                        result = string.CompareOrdinal(ma, mb);
                        // Master名が同じならOffsetでサブソート
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
