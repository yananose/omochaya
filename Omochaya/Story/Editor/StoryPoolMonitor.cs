// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StoryTask.cs" company="Omochaya">
//   Copyright (c) 2026 Omochaya. All rights reserved.
//   Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// <summary>
//   Provides a custom Unity EditorWindow interface to visualize, monitor, and profile real-time
//   memory footprints and allocation capacities of active object pools within the framework.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#if (FOR_DEBUG || UNITY_EDITOR) && !STORY_FAST

// 〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜
// これ以降は間接的に使用されます。利用者が直接使用することは想定していません
// 〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜
namespace Omochaya.HiddenStory
{
    using UnityEditor;
    using UnityEngine;
    using System.Linq;
    using System.Collections.Generic;

    class PoolMonitorForDebug : EditorWindow
    {
        Vector2 scrollPosition;

        // ★ ソート用の状態管理
        enum SortColumn { Name, Active, Worst, Free, MemorySize }
        SortColumn currentSortColumn = SortColumn.Name;
        bool sortAscending = true;

        /// <summary>Opens or focuses the custom Omochaya Pool Monitor tracking window in the Unity Editor.</summary>
        [MenuItem("Window/Omochaya/Pool Monitor")]
        public static void ShowWindow()
        {
            var window = GetWindow<PoolMonitorForDebug>("Pool Monitor");
            window.Show();
        }

        void OnInspectorUpdate()
        {
            Repaint();
        }

        void Update()
        {
            if (!Application.isPlaying || EditorApplication.isPaused) return;
            foreach (var monitor in IPoolMonitorForDebug.Monitors) { monitor.WorstCount = Mathf.Max(monitor.WorstCount, monitor.ActiveCount); }
        }

        void OnGUI()
        {
            GUILayout.Space(10);
            GUILayout.Label("Omochaya Pool Monitor", EditorStyles.boldLabel);
            GUILayout.Space(5);

            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox(Messages.EditorUI.PoolMonitor_PlayModeOnly, MessageType.Info);
            }

            // テーブルのヘッダー描画（ここでクリック入力を受け付ける）
            DrawHeader();

            // ★ Linq を使って表示用にソート
            IEnumerable<IPoolMonitorForDebug> sortedMonitors = IPoolMonitorForDebug.Monitors.Where(m => m != null);
            switch (currentSortColumn)
            {
                case SortColumn.Name:
                    sortedMonitors = sortAscending ? sortedMonitors.OrderBy(m => m.PoolName) : sortedMonitors.OrderByDescending(m => m.PoolName);
                    break;
                case SortColumn.Active:
                    sortedMonitors = sortAscending ? sortedMonitors.OrderBy(m => m.ActiveCount) : sortedMonitors.OrderByDescending(m => m.ActiveCount);
                    break;
                case SortColumn.Worst:
                    sortedMonitors = sortAscending ? sortedMonitors.OrderBy(m => m.WorstCount) : sortedMonitors.OrderByDescending(m => m.WorstCount);
                    break;
                case SortColumn.Free:
                    sortedMonitors = sortAscending ? sortedMonitors.OrderBy(m => m.FreeCount) : sortedMonitors.OrderByDescending(m => m.FreeCount);
                    break;
                case SortColumn.MemorySize:
                    sortedMonitors = sortAscending ? sortedMonitors.OrderBy(m => m.TotalBytes) : sortedMonitors.OrderByDescending(m => m.TotalBytes);
                    break;
            }

            // スクロールビュー開始
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            long grandTotalBytes = 0;
            double grandTotalActiveBytes = 0;

            // ソート済みのリストをループ処理
            foreach (var monitor in sortedMonitors)
            {
                DrawRow(
                    monitor.PoolName,
                    monitor.ActiveCount.ToString(),
                    monitor.WorstCount.ToString(),
                    monitor.FreeCount.ToString(),
                    DevForEditor.FormatMemorySize(monitor.TotalBytes)
                );

                grandTotalBytes += monitor.TotalBytes;
                grandTotalActiveBytes += monitor.TotalBytes * (monitor.ActiveCount / (double)(monitor.ActiveCount + monitor.FreeCount));
            }

            EditorGUILayout.EndScrollView();

            // フッター（合計値の表示）
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(string.Format(Messages.EditorUI.PoolMonitor_StatCount, IPoolMonitorForDebug.Monitors.Count), EditorStyles.boldLabel, GUILayout.Width(120));
            GUILayout.Label(string.Format(Messages.EditorUI.PoolMonitor_StatUsage, grandTotalActiveBytes/grandTotalBytes), EditorStyles.boldLabel, GUILayout.Width(180));
            GUILayout.Label(string.Format(Messages.EditorUI.PoolMonitor_StatMemory, DevForEditor.FormatMemorySize((int)Mathf.Min(grandTotalBytes, int.MaxValue))), EditorStyles.boldLabel, GUILayout.Width(200));
            EditorGUILayout.EndHorizontal();
            
            GUILayout.Space(10);
        }

        void DrawHeader()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            
            // ★ ラベルをボタンに変更し、クリックでソート状態を切り替え
            if (GUILayout.Button(GetHeaderLabel(Messages.EditorUI.PoolMonitor_ColActive, SortColumn.Active), EditorStyles.toolbarButton, GUILayout.Width(60)))
                ToggleSort(SortColumn.Active);

            if (GUILayout.Button(GetHeaderLabel(Messages.EditorUI.PoolMonitor_ColWorst, SortColumn.Worst), EditorStyles.toolbarButton, GUILayout.Width(60)))
                ToggleSort(SortColumn.Worst);

            if (GUILayout.Button(GetHeaderLabel(Messages.EditorUI.PoolMonitor_ColFree, SortColumn.Free), EditorStyles.toolbarButton, GUILayout.Width(60)))
                ToggleSort(SortColumn.Free);

            if (GUILayout.Button(GetHeaderLabel(Messages.EditorUI.PoolMonitor_ColMemory, SortColumn.MemorySize), EditorStyles.toolbarButton, GUILayout.Width(100)))
                ToggleSort(SortColumn.MemorySize);

            if (GUILayout.Button(GetHeaderLabel(Messages.EditorUI.PoolMonitor_ColName, SortColumn.Name), EditorStyles.toolbarButton, GUILayout.Width(460)))
                ToggleSort(SortColumn.Name);

            EditorGUILayout.EndHorizontal();
        }

        void DrawRow(string name, string active, string worst, string free, string memory)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(active, GUILayout.Width(55));
            GUILayout.Label(worst, GUILayout.Width(55));
            GUILayout.Label(free, GUILayout.Width(55));
            GUILayout.Label(memory, GUILayout.Width(100));
            GUILayout.Label(name, GUILayout.Width(460));
            EditorGUILayout.EndHorizontal();
        }

        // --- ソート用ヘルパー ---

        void ToggleSort(SortColumn column)
        {
            if (currentSortColumn == column)
            {
                // 同じ列をクリックしたら昇順/降順を反転
                sortAscending = !sortAscending;
            }
            else
            {
                // 別の列をクリックしたら、その列を対象にしてデフォルトの向きにする
                currentSortColumn = column;
                // メモリやカウントは最初から降順（大きい順）で見たいことが多いので、名前以外は降順をデフォルトにする
                sortAscending = column == SortColumn.Name;
            }
        }

        string GetHeaderLabel(string text, SortColumn column)
        {
            if (currentSortColumn == column)
            {
                return text + (sortAscending ? " ▲" : " ▼");
            }
            return text;
        }
    }
}
#endif