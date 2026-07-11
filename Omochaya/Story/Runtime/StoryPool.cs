// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StoryPool.cs" company="Omochaya">
//   Copyright (c) 2026 Omochaya. All rights reserved.
//   Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// <summary>
//   Implements highly optimized generation-aware object pools, managing unified single-type arrays
//   and cache-efficient split hot/cool structures.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Omochaya
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using HiddenStory;

    public static partial class Story
    {
        public static class Pool
        {
            /// <summary>Represents a unique identifier for a pooled resource, combining an array index and a generation age to ensure safe access.</summary>
            // プール（構造体配列）のId。
            // IndexとAgeを組み合わせて、IDの有効性を検証する。
            // Idの割り当てと解放はPool.Coreを使用して行われる。
            public readonly struct Id : IEquatable<Id>
            {
                // fields

                /// <summary>The internal array index and validation generation age of this identifier.</summary>
                public readonly int Index, Age;

                // constructors

                [MethodImpl(MethodImplOptions.AggressiveInlining)] // サイズが小さい
                public Id(int index, int age) { this.Index = index; this.Age = age; }

                // properties

                /// <summary>Gets a value indicating whether this identifier is valid and assigned.</summary>
                public readonly bool IsValid
                {
                    [MethodImpl(MethodImplOptions.AggressiveInlining)] // サイズが小さい
                    get => this.Age != 0;
                }

                // methods

                /// <summary>Determines whether this identifier matches the specified identifier.</summary>
                public readonly bool Matches(Id a) => this.Index == a.Index && this.Age == a.Age;

                // for collection（ユーザによる呼び出し禁止）

                /// <summary>Don't touch! Only for system.</summary>
                public bool Equals(Id other) => Matches(other);

                /// <summary>Don't touch! Only for system.</summary>
                public override bool Equals(object obj) => obj is Id other && Equals(other);

                /// <summary>Don't touch! Only for system.</summary>
                public override int GetHashCode() => HashCode.Combine(Index, Age);
            }

            const int CREATE_LIMIT_SIZE = 1024 * 1;
            const int EXPAND_LIMIT_SIZE = 1024 * 128;

            /// <summary></summary>
            // [MethodImpl(MethodImplOptions.AggressiveInlining)] // コンパイラに任せる
            public static void Expand<T>(ref T[] array, int count)
            {
                if (array == null) { array = new T[count]; }
                else
                {
                    Dev.Assert(array.Length < count);
                    Dev.LogWarning(string.Format(Messages.Warnings.ArrayExpanded, array.Length, count, Dev.FormatMemorySize(Unsafe.SizeOf<T>() * count), typeof(T).Name));
                    System.Array.Resize(ref array, count);
                }
            }

            /// <summary>Calculates the optimal initial element capacity bounded by the specified memory limit threshold.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)] // サイズが小さい
            public static int GetNeedCountAtCreate(int itemSize, int limit = CREATE_LIMIT_SIZE)
                => Mathf.Clamp(limit / itemSize, 8, 32);

            /// <summary>Calculates the optimal expanded element capacity scaled geometrically and bounded by the specified memory limit threshold.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)] // サイズが小さい
            public static int GetNeedCountAtExpand(int count, int itemSize, int limit = EXPAND_LIMIT_SIZE)
                => count + Mathf.Min(count, Mathf.Max(4, limit / itemSize));

#if (FOR_DEBUG || UNITY_EDITOR) && !STORY_NO_DEBUG
            // ※ GetNeedCountAtCreate を public に変更したと仮定したメッセージです。
            // 変更しない場合はメッセージの実装指示を書き換えてください。
            [Obsolete("このメソッドは廃止されました。代わりに Expand(ref array, GetNeedCountAtCreate(Unsafe.SizeOf<T>())) を使用してください。", true)]
            public static void Create<T>(ref T[] array)
                => throw new NotSupportedException();

            [Obsolete("このメソッドは廃止されました。代わりに Expand(ref array, GetNeedCountAtExpand(array.Length, Unsafe.SizeOf<T>())) を使用してください。", true)]
            public static void Expand<T>(ref T[] array)
                => throw new NotSupportedException();

            [Obsolete("このメソッドは廃止されました。代わりに Expand(ref array, GetNeedCountAtCreate(itemSize, limit)) を使用してください。", true)]
            public static void CreateBasedOnItemSize<T>(ref T[] array, int itemSize, int limit = CREATE_LIMIT_SIZE)
                => throw new NotSupportedException();

            [Obsolete("このメソッドは廃止されました。代わりに Expand(ref array, GetNeedCountAtExpand(array.Length, itemSize, limit)) を使用してください。", true)]
            public static void ExpandBasedOnItemSize<T>(ref T[] array, int itemSize, int limit = EXPAND_LIMIT_SIZE)
                => throw new NotSupportedException();
#endif

        }

        // 構造体配列によるプールの実装。IDによる要素管理とロック機構を提供する。
        /// <summary>A standard single-type instance pool managed through static shared access.</summary>
        public class Pool<T> : PoolBase<T>
        {
            /// <summary>The globally shared singleton instance of this single-type pool.</summary>
            public static readonly Pool<T> Shared = new();
            Pool() { }

#if (FOR_DEBUG || UNITY_EDITOR) && !STORY_NO_DEBUG
            /// <summary>Don't touch! Only for system.</summary>
            public override string PoolName => Dev.Pool<T>.Name;
            /// <summary>Don't touch! Only for system.</summary>
            public override int TotalBytes => ArraySize + Unsafe.SizeOf<Pool<T>>();
#endif
        }

        /// <summary>A split-data instance pool managing hot and cool data structures for enhanced cache efficiency.</summary>
        public class Pool<HOT, COOL> : PoolBase<HOT, COOL>
        {
            /// <summary>The globally shared singleton instance of this split hot/cool data pool.</summary>
            public static readonly Pool<HOT, COOL> Shared = new();
            Pool() { }

#if (FOR_DEBUG || UNITY_EDITOR) && !STORY_NO_DEBUG
            /// <summary>Don't touch! Only for system.</summary>
            public override string PoolName => Dev.Pool<HOT, COOL>.Name;
            /// <summary>Don't touch! Only for system.</summary>
            public override int TotalBytes => ArraySize + Unsafe.SizeOf<Pool<HOT, COOL>>();
#endif
        }

        /// <summary>Represents a lightweight, generation-free memory handle for safely accessing unmanaged pooled resources.</summary>
        // 世代管理しないプールで比較的安全に要素にアクセスするための構造体。
        // コンストラクタで指定されたプールから要素を確保し index を隠蔽してアクセスする。
        // Dispose（あるいは Free）を怠ると容易にリークするので注意。
        // コピーされた場合はどちらかで解放するともう片方でそれを検知することはできない！
        // コピーは極力避け、どうしても必要な場合はどちらが主体かを明確にすること。
        // その場合は強参照と弱参照の関係になるので、主体ではない方は主体が解放していないことが保証されている場合にのみ使用すること。
        public readonly struct PoolMemory : IDisposable
        {
            // fields

            readonly IUnsafePool pool;
            readonly int index;

            // properties

            /// <summary>Gets a value indicating whether this memory handle is bound to an active unmanaged pool.</summary>
            public bool IsValid
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)] // サイズが小さい
                get => this.pool != null;
            }

            // constructors

            [MethodImpl(MethodImplOptions.AggressiveInlining)] // サイズが小さい
            PoolMemory(IUnsafePool pool, int index) { this.pool = pool; this.index = index; }

            // methods

            /// <summary></summary>
            public void Expand<T>(int count)
                => UnsafePool<T>.Shared.Expand(count);

            /// <summary>Allocates a lightweight memory handle from the hidden global shared pool without generation tracking.</summary>
            public static PoolMemory Alloc<T>()
            {
                var pool = UnsafePool<T>.Shared;
                return new PoolMemory(pool, pool.Alloc());
            }

            /// <summary>Allocates a lightweight memory handle initialized with a value from the hidden global shared pool without generation tracking.</summary>
            public static PoolMemory Alloc<T>(in T value)
            {
                var pool = UnsafePool<T>.Shared;
                return new PoolMemory(pool, pool.Alloc(value));
            }

            /// <summary>Explicitly releases the unmanaged memory slot handle back to its originating pool.</summary>
            public void Free()
            {
                if (IsValid) { this.pool.Free(this.index); }
            }

            /// <summary>Gets a reference to the unmanaged value from the hidden global shared pool.</summary>
            /// <remarks>
            /// <b>WARNING: Reference Invalidation Risk</b><br/>
            /// Do not hold the returned <c>ref T</c> in a local variable across calls to <c>Alloc()</c> on the same pool.<br/>
            /// If the pool expands (resizes) its internal array, the held reference will point to invalid memory, leading to critical bugs or data loss.<br/>
            /// <br/>
            /// <i>Note: <c>Get()</c> is highly optimized and lightweight. Always re-fetch the reference after any operation that might allocate.</i>
            /// </remarks>
            /// <example>
            /// <code>
            /// // ❌ BAD:
            /// ref var item = ref memory.Get();
            /// pool.Alloc(); // Expansion might invalidate 'item'!
            /// item.Value = 10;
            /// 
            /// // ⭕ GOOD:
            /// memory.Get().Value = 10;
            /// pool.Alloc();
            /// memory.Get().Value = 20;
            /// </code>
            /// </example>
            public ref T Get<T>()
            {
                Dev.Assert(UnsafePool<T>.Shared == this.pool, string.Format("{0} != {1}. type is {2}", UnsafePool<T>.Shared, this.pool, typeof(T)));
                return ref UnsafePool<T>.Shared.Get(this.index);
            }

            /// <summary>Determines whether the allocated memory type does not match the specified type.</summary>
            // 型が一致しない
            public bool IsMissType<T>() => UnsafePool<T>.Shared != this.pool;

            /// <summary>Disposes of the unmanaged memory handle, safely recycling its index slot.</summary>
            public void Dispose() => Free();

#if (FOR_DEBUG || UNITY_EDITOR) && !STORY_NO_DEBUG
            public override string ToString() => this.pool != null ? this.pool.Name : "(empty)";
#endif
        }

        /// <summary>Don't touch! Only for system.（継承しないでください）</summary>
        // プールの基底クラス。IDによる要素へのアクセスを実現する。
        // IdにAgeをもちIdの最終的な有効性を IsValid により判定する。
        public abstract class PoolBase<T>
#if (FOR_DEBUG || UNITY_EDITOR) && !STORY_NO_DEBUG
            : IPoolMonitorForDebug
#endif
        {
            // inner classes

            /// <summary>Don't touch! Only for system.</summary>
            // スロット配列。Slot構造体の配列を管理し、IDの割り当てと解放、要素へのアクセスを提供する。
            protected struct PoolSlotArray
            {
                PoolSlot<T>[] array;

                /// <summary>Expands the underlying slot array to the specified capacity.</summary>
                [MethodImpl(MethodImplOptions.AggressiveInlining)] // サイズが小さい
                public void Expand(int count) => Pool.Expand(ref this.array, count);

                /// <summary>Allocates a new identifier from the underlying slot at the specified index.</summary>
                [MethodImpl(MethodImplOptions.AggressiveInlining)] // サイズが小さい
                public Pool.Id Alloc(int index) => this.array[index].Alloc(index);

                /// <summary>Frees the slot at the specified index and clears its value.</summary>
                [MethodImpl(MethodImplOptions.AggressiveInlining)] // サイズが小さい
                public void Free(int index) => this.array[index].Free();

                /// <summary>Validates whether the given identifier matches the index and generation age of the active slot.</summary>
                [MethodImpl(MethodImplOptions.AggressiveInlining)] // サイズが小さい
                public bool IsValid(Pool.Id id) => this.array != null && (uint)id.Index < this.array.Length && this.array[id.Index].Age == id.Age;

                /// <summary>Gets a reference to the value associated with the specified identifier.</summary>
                /// <remarks>
                /// <b>WARNING: Reference Invalidation Risk</b><br/>
                /// Do not hold the returned <c>ref T</c> in a local variable across calls to <c>Alloc()</c> on the same pool.<br/>
                /// If the pool expands (resizes) its internal array, the held reference will point to invalid memory, leading to critical bugs or data loss.<br/>
                /// <br/>
                /// <i>Note: <c>Get()</c> is highly optimized and lightweight. Always re-fetch the reference after any operation that might allocate.</i>
                /// </remarks>
                /// <example>
                /// <code>
                /// // ❌ BAD:
                /// ref var item = ref pool.Get(id);
                /// pool.Alloc(); // Expansion might invalidate 'item'!
                /// item.Value = 10;
                /// 
                /// // ⭕ GOOD:
                /// pool.Get(id).Value = 10;
                /// pool.Alloc();
                /// pool.Get(id).Value = 20;
                /// </code>
                /// </example>
                [MethodImpl(MethodImplOptions.AggressiveInlining)] // サイズが小さい
                public ref T Get(Pool.Id id)
                {
                    Dev.Assert(IsValid(id));
                    return ref this.array[id.Index].Value;
                }

                /// <summary>Gets a direct reference to the value at the raw index without validation.</summary>
                /// <remarks>
                /// <b>WARNING: Reference Invalidation Risk</b><br/>
                /// Do not hold the returned <c>ref T</c> in a local variable across calls to <c>Alloc()</c> on the same pool.<br/>
                /// If the pool expands (resizes) its internal array, the held reference will point to invalid memory, leading to critical bugs or data loss.<br/>
                /// <br/>
                /// <i>Note: <c>UnsafeGet()</c> is highly optimized and lightweight. Always re-fetch the reference after any operation that might allocate.</i>
                /// </remarks>
                /// <example>
                /// <code>
                /// // ❌ BAD:
                /// ref var item = ref pool.UnsafeGet(index);
                /// pool.Alloc(); // Expansion might invalidate 'item'!
                /// item.Value = 10;
                /// 
                /// // ⭕ GOOD:
                /// pool.UnsafeGet(index).Value = 10;
                /// pool.Alloc();
                /// pool.UnsafeGet(index).Value = 20;
                /// </code>
                /// </example>
                [MethodImpl(MethodImplOptions.AggressiveInlining)] // サイズが小さい
                public ref T UnsafeGet(int index)
                    => ref this.array[index].Value;

                /// <summary>Retrieves the current identifier for the raw index using its active generation age.</summary>
                [MethodImpl(MethodImplOptions.AggressiveInlining)] // サイズが小さい
                public Pool.Id UnsafeGetId(int index)
                    => new Pool.Id(index, this.array[index].Age);

                /// <summary>Forcibly updates the generation age of the slot matching the specified identifier.</summary>
                [MethodImpl(MethodImplOptions.AggressiveInlining)] // サイズが小さい
                public void Reborn(Pool.Id id)
                {
                    Dev.Assert(IsValid(id));
                    this.array[id.Index].Reborn();
                }

                /// <summary>Forcibly updates the generation age of the slot at the raw index without validation.</summary>
                [MethodImpl(MethodImplOptions.AggressiveInlining)] // サイズが小さい
                public void UnsafeReborn(int index) => this.array[index].Reborn();

#if (FOR_DEBUG || UNITY_EDITOR) && !STORY_NO_DEBUG
                public int ArraySize => Unsafe.SizeOf<PoolSlot<T>>() * (this.array?.Length ?? 0);
#endif
            }

            // fields
            private protected PoolCore core;

            /// <summary>Don't touch! Only for system.</summary>
            protected PoolSlotArray array;

            // constructors
            protected PoolBase() => Dev.PoolMonitorRegister(this); // 直接 new させない

            // properties

            /// <summary>Gets the current length of the active core allocation pool.</summary>
            public int Length => this.core.Count;

            // methods

            /// <summary>Expands the pool capacity to the specified count if it exceeds the current capacity.</summary>
            public void Expand(int count)
            {
                if (this.core.TryExpand(count)) { this.array.Expand(count); }
            }

            /// <summary>Allocates an empty slot from the pool and returns its unique identifier.</summary>
            public Pool.Id Alloc()
            {
                if (!this.core.TryAllocFast(out int index))
                {
                    var result = this.core.ExpandAndAlloc(Unsafe.SizeOf<PoolSlot<T>>());
                    if (0 < result.Need) { this.array.Expand(result.Need); }
                    index = result.Index;
                }
                return this.array.Alloc(index);
            }

            /// <summary>Safely releases the slot associated with the specified identifier back to the pool.</summary>
            public void Free(Pool.Id id)
            {
                if (!IsValid(id)) { return; }
                this.array.Free(id.Index);
                this.core.Free(id.Index);
            }

            /// <summary>Releases the slot at the raw index back to the pool without identifier validation.</summary>
            public void UnsafeFree(int index)
            {
                this.array.Free(index);
                this.core.Free(index);
            }

            /// <summary>Determines whether the specified identifier is valid within the underlying slot array.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)] // サイズが小さい
            public bool IsValid(Pool.Id id) => this.array.IsValid(id);

            /// <summary>Gets a reference to the element associated with the valid identifier.</summary>
            /// <remarks>
            /// <b>WARNING: Reference Invalidation Risk</b><br/>
            /// Do not hold the returned <c>ref T</c> in a local variable across calls to <c>Alloc()</c> on the same pool.<br/>
            /// If the pool expands (resizes) its internal array, the held reference will point to invalid memory, leading to critical bugs or data loss.<br/>
            /// <br/>
            /// <i>Note: <c>Get()</c> is highly optimized and lightweight. Always re-fetch the reference after any operation that might allocate.</i>
            /// </remarks>
            /// <example>
            /// <code>
            /// // ❌ BAD:
            /// ref var item = ref pool.Get(id);
            /// pool.Alloc(); // Expansion might invalidate 'item'!
            /// item.Value = 10;
            /// 
            /// // ⭕ GOOD:
            /// pool.Get(id).Value = 10;
            /// pool.Alloc();
            /// pool.Get(id).Value = 20;
            /// </code>
            /// </example>
            [MethodImpl(MethodImplOptions.AggressiveInlining)] // サイズが小さい
            public ref T Get(Pool.Id id) => ref this.array.Get(id);

            /// <summary>Gets a direct reference to the element at the raw index without identifier validation.</summary>
            /// <remarks>
            /// <b>WARNING: Reference Invalidation Risk</b><br/>
            /// Do not hold the returned <c>ref T</c> in a local variable across calls to <c>Alloc()</c> on the same pool.<br/>
            /// If the pool expands (resizes) its internal array, the held reference will point to invalid memory, leading to critical bugs or data loss.<br/>
            /// <br/>
            /// <i>Note: <c>UnsafeGet()</c> is highly optimized and lightweight. Always re-fetch the reference after any operation that might allocate.</i>
            /// </remarks>
            /// <example>
            /// <code>
            /// // ❌ BAD:
            /// ref var item = ref pool.UnsafeGet(index);
            /// pool.Alloc(); // Expansion might invalidate 'item'!
            /// item.Value = 10;
            /// 
            /// // ⭕ GOOD:
            /// pool.UnsafeGet(index).Value = 10;
            /// pool.Alloc();
            /// pool.UnsafeGet(index).Value = 20;
            /// </code>
            /// </example>
            [MethodImpl(MethodImplOptions.AggressiveInlining)] // サイズが小さい
            public ref T UnsafeGet(int index) => ref this.array.UnsafeGet(index);

            /// <summary>Retrieves the identifier corresponding to the raw index from the underlying slot array.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)] // サイズが小さい
            public Pool.Id UnsafeGetId(int index) => this.array.UnsafeGetId(index);

            /// <summary>Advances the generation age of the slot associated with the specified identifier.</summary>
            public void Reborn(Pool.Id id) => this.array.Reborn(id);

            /// <summary>Advances the generation age of the slot at the raw index without validation.</summary>
            public void UnsafeReborn(int index) => this.array.UnsafeReborn(index);

#if (FOR_DEBUG || UNITY_EDITOR) && !STORY_NO_DEBUG
            /// <summary>Don't touch! Only for system.</summary>
            public int ActiveCount => this.core.ActiveCount;
            /// <summary>Don't touch! Only for system.</summary>
            public int WorstCount => this.core.WorstCount;
            /// <summary>Don't touch! Only for system.</summary>
            public int FreeCount => this.core.TotalCount - this.core.ActiveCount;
            /// <summary>Don't touch! Only for system.</summary>
            public abstract string PoolName { get; }
            /// <summary>Don't touch! Only for system.</summary>
            public abstract int TotalBytes { get; }
            /// <summary>Don't touch! Only for system.</summary>
            protected int ArraySize => this.array.ArraySize + this.core.ArraySize;

            [Obsolete("このメソッドは廃止されました。代わりに Get(Alloc()) = value を使用してください。", true)]
            public Pool.Id Alloc(in T value)
                => throw new NotSupportedException();
#endif
        }

        /// <summary>Don't touch! Only for system.（継承しないでください）</summary>
        // 2つの型を管理するプールの基底クラス。PoolBase<T>を継承し、追加のデータ配列を管理する。
        // hot なデータを分離することでキャッシュ効率の向上を狙う。
        public abstract class PoolBase<HOT, COOL> : PoolBase<COOL>
        {
            // fields
            protected HOT[] hotArray;

            // constructors
            protected PoolBase() { } // 直接 new させない

            // methods

            /// <summary>Expands both the hot and cool data arrays to the specified count.</summary>
            public new void Expand(int count)
            {
                if (this.core.TryExpand(count))
                {
                    Pool.Expand(ref this.hotArray, count);
                    this.array.Expand(count); // for cool
                }
            }

            /// <summary>Allocates a slot and returns its identifier, splitting management into hot and cool data arrays.</summary>
            public new Pool.Id Alloc()
            {
                if (!this.core.TryAllocFast(out int index))
                {
                    var result = this.core.ExpandAndAlloc(Unsafe.SizeOf<HOT>() + Unsafe.SizeOf<PoolSlot<COOL>>());
                    Pool.Expand(ref this.hotArray, result.Need);
                    this.array.Expand(result.Need); // for cool
                    index = result.Index;
                }
                return this.array.Alloc(index);
            }

            /// <summary>Safely releases the slot and resets its associated hot and cool elements using the given identifier.</summary>
            public new void Free(Pool.Id id)
            {
                if (!IsValid(id)) { return; }
                UnsafeFree(id.Index);
            }

            /// <summary>Releases the slot at the raw index without validation, resetting its hot and cool data.</summary>
            public new void UnsafeFree(int index)
            {
                base.UnsafeFree(index);
                this.hotArray[index] = default;
            }

            /// <summary>Gets a reference to the hot data associated with the specified identifier.</summary>
            /// <remarks>
            /// <b>WARNING: Reference Invalidation Risk</b><br/>
            /// Do not hold the returned <c>ref T</c> in a local variable across calls to <c>Alloc()</c> on the same pool.<br/>
            /// If the pool expands (resizes) its internal array, the held reference will point to invalid memory, leading to critical bugs or data loss.<br/>
            /// <br/>
            /// <i>Note: <c>Get()</c> is highly optimized and lightweight. Always re-fetch the reference after any operation that might allocate.</i>
            /// </remarks>
            /// <example>
            /// <code>
            /// // ❌ BAD:
            /// ref var item = ref pool.Get(id);
            /// pool.Alloc(); // Expansion might invalidate 'item'!
            /// item.Value = 10;
            /// 
            /// // ⭕ GOOD:
            /// pool.Get(id).Value = 10;
            /// pool.Alloc();
            /// pool.Get(id).Value = 20;
            /// </code>
            /// </example>
            [MethodImpl(MethodImplOptions.AggressiveInlining)] // サイズが小さい
            public new ref HOT Get(Pool.Id id)
            {
                Dev.Assert(IsValid(id));
                return ref this.hotArray[id.Index];
            }

            /// <summary>Gets a direct reference to the hot data associated at the raw index without identifier validation.</summary>
            /// <remarks>
            /// <b>WARNING: Reference Invalidation Risk</b><br/>
            /// Do not hold the returned <c>ref T</c> in a local variable across calls to <c>Alloc()</c> on the same pool.<br/>
            /// If the pool expands (resizes) its internal array, the held reference will point to invalid memory, leading to critical bugs or data loss.<br/>
            /// <br/>
            /// <i>Note: <c>Get()</c> is highly optimized and lightweight. Always re-fetch the reference after any operation that might allocate.</i>
            /// </remarks>
            /// <example>
            /// <code>
            /// // ❌ BAD:
            /// ref var item = ref pool.UnsafeGet(index);
            /// pool.Alloc(); // Expansion might invalidate 'item'!
            /// item.Value = 10;
            /// 
            /// // ⭕ GOOD:
            /// pool.UnsafeGet(index).Value = 10;
            /// pool.Alloc();
            /// pool.UnsafeGet(index).Value = 20;
            /// </code>
            /// </example>
            [MethodImpl(MethodImplOptions.AggressiveInlining)] // サイズが小さい
            public new ref HOT UnsafeGet(int index) => ref this.hotArray[index];

            /// <summary>Gets a reference to the cool data associated with the specified identifier.</summary>
            /// <remarks>
            /// <b>WARNING: Reference Invalidation Risk</b><br/>
            /// Do not hold the returned <c>ref T</c> in a local variable across calls to <c>Alloc()</c> on the same pool.<br/>
            /// If the pool expands (resizes) its internal array, the held reference will point to invalid memory, leading to critical bugs or data loss.<br/>
            /// <br/>
            /// <i>Note: <c>Get2()</c> is highly optimized and lightweight. Always re-fetch the reference after any operation that might allocate.</i>
            /// </remarks>
            /// <example>
            /// <code>
            /// // ❌ BAD:
            /// ref var item = ref pool.Get2(id);
            /// pool.Alloc(); // Expansion might invalidate 'item'!
            /// item.Value = 10;
            /// 
            /// // ⭕ GOOD:
            /// pool.Get2(id).Value = 10;
            /// pool.Alloc();
            /// pool.Get2(id).Value = 20;
            /// </code>
            /// </example>
            [MethodImpl(MethodImplOptions.AggressiveInlining)] // サイズが小さい
            public ref COOL Get2(Pool.Id id) => ref base.Get(id);

            /// <summary>Gets a direct reference to the cool data associated at the raw index without identifier validation.</summary>
            /// <remarks>
            /// <b>WARNING: Reference Invalidation Risk</b><br/>
            /// Do not hold the returned <c>ref T</c> in a local variable across calls to <c>Alloc()</c> on the same pool.<br/>
            /// If the pool expands (resizes) its internal array, the held reference will point to invalid memory, leading to critical bugs or data loss.<br/>
            /// <br/>
            /// <i>Note: <c>UnsafeGet2()</c> is highly optimized and lightweight. Always re-fetch the reference after any operation that might allocate.</i>
            /// </remarks>
            /// <example>
            /// <code>
            /// // ❌ BAD:
            /// ref var item = ref pool.UnsafeGet2(index);
            /// pool.Alloc(); // Expansion might invalidate 'item'!
            /// item.Value = 10;
            /// 
            /// // ⭕ GOOD:
            /// pool.UnsafeGet2(index).Value = 10;
            /// pool.Alloc();
            /// pool.UnsafeGet2(index).Value = 20;
            /// </code>
            /// </example>
            [MethodImpl(MethodImplOptions.AggressiveInlining)] // サイズが小さい
            public ref COOL UnsafeGet2(int index) => ref base.UnsafeGet(index);

#if (FOR_DEBUG || UNITY_EDITOR) && !STORY_NO_DEBUG
            /// <summary>Don't touch! Only for system.</summary>
            protected new int ArraySize => base.ArraySize + Unsafe.SizeOf<COOL>() * (ActiveCount + FreeCount);

            [Obsolete("このメソッドは廃止されました。代わりに var id = Alloc(); Get(id) = hot; Get2(id) = cool; を使用してください。", true)]
            public Pool.Id Alloc(in HOT hot, in COOL cool)
                => throw new NotSupportedException();
#endif
        }


        // 〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜
        // これ以降は間接的に使用されます。利用者が直接使用することは想定していません
        // 〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜

        /// <summary>Don't touch! Only for system.</summary>
        // プールのコア機能。
        // 要素の確保と解放、ロック管理を担当する。
        // IDのうちIndexを管理し、必要に応じてプールの拡張も行う。
        internal struct PoolCore
        {
            // inner classes

            /// <summary>Don't touch! Only for system.</summary>
            internal readonly struct Result
            {
                /// <summary>Don't touch! Only for system.</summary>
                internal readonly int Need;
                /// <summary>Don't touch! Only for system.</summary>
                internal readonly int Index;
                /// <summary>Don't touch! Only for system.</summary>
                [MethodImpl(MethodImplOptions.AggressiveInlining)] // サイズが小さい
                internal Result(int need, int index) { this.Need = need; this.Index = index; }
            }

            // fields

            int[] nextFree;
            int freeHead;

#if (FOR_DEBUG || UNITY_EDITOR) && !STORY_NO_DEBUG
            int useCount;
            int worstCount;
#endif

            // properties

            /// <summary>Don't touch! Only for system.</summary>
            internal readonly bool IsValid
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)] // サイズが小さい
                get => this.nextFree != null;
            }

            /// <summary>Don't touch! Only for system.</summary>
            internal readonly int Count
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)] // サイズが小さい
                get => this.nextFree?.Length ?? 0;
            }

#if (FOR_DEBUG || UNITY_EDITOR) && !STORY_NO_DEBUG
            /// <summary>Don't touch! Only for system.</summary>
            internal readonly int ActiveCount => this.useCount;
            /// <summary>Don't touch! Only for system.</summary>
            internal readonly int WorstCount => this.worstCount;
            /// <summary>Don't touch! Only for system.</summary>
            internal readonly int TotalCount => this.nextFree?.Length ?? 0;
            /// <summary>Don't touch! Only for system.</summary>
            internal readonly int ArraySize => Unsafe.SizeOf<int>() * TotalCount;
#endif

            // methods

            // *************************************************************
            // *** フリーリストをジェネリクスさせずに切り分けているのは、
            // *** この MakeLinkをブロートさせないため。
            // *** 切り分けた以上、Core の呼び出しは最小限になるように実装すること。
            // *************************************************************
            // [MethodImpl(MethodImplOptions.AggressiveInlining)] // MakeLink使用メソッドを NoInlining にしているのでここは Aggressive でもいいが...コンパイラに任せる
            void MakeLink(int oldCount)
            {
                var newCount = this.nextFree.Length;
                for (var i = oldCount; i < newCount-1; ++i) { this.nextFree[i] = i + 1; }
                this.nextFree[newCount - 1] = this.freeHead;
                this.freeHead = oldCount;
            }

            /// <summary>Don't touch! Only for system.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)] // サイズが小さい（メソッド呼び出しと大差ないはず）
            internal void Free(int index)
            {
                this.nextFree[index] = this.freeHead;
                this.freeHead = index;

#if (FOR_DEBUG || UNITY_EDITOR) && !STORY_NO_DEBUG
                this.useCount--;
#endif

            }

#if !STORY_NO_PRE_CAPACITY
            /// <summary>Don't touch! Only for system.</summary>
            [MethodImpl(MethodImplOptions.NoInlining)] // !!! ジェネリクスによるコードブロート防止のため明示的にインライン化しない !!!
            internal void FirstAlloc(int count)
            {
                this.freeHead = -1;
                this.nextFree = new int[count];
                MakeLink(0);

                this.freeHead = 1;

#if (FOR_DEBUG || UNITY_EDITOR) && !STORY_NO_DEBUG
                this.useCount = 1;
                this.worstCount = 1;
#endif
            }
#endif

            /// <summary>Don't touch! Only for system.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)] // （速度優先のためこのインライン化だけは許容...）
            internal bool TryAllocFast(out int index)
            {
                index = this.freeHead;
                if (this.nextFree != null && index != -1)
                {
                    this.freeHead = this.nextFree[index];
#if (FOR_DEBUG || UNITY_EDITOR) && !STORY_NO_DEBUG
                    this.useCount++;
                    this.worstCount = Mathf.Max(this.worstCount, this.useCount);
#endif
                    return true;
                }
                return false;
            }

            /// <summary>Don't touch! Only for system.</summary>
            [MethodImpl(MethodImplOptions.NoInlining)] // !!! ジェネリクスによるコードブロート防止のため明示的にインライン化しない !!!
            internal Result ExpandAndAlloc(int itemSize)
            {
                int need;

                // expand
                if (this.nextFree == null)
                {
                    need = Pool.GetNeedCountAtCreate(itemSize);
                    this.freeHead = -1;
                    this.nextFree = new int[need];
                    MakeLink(0);
                }
                else
                {
                    need = Pool.GetNeedCountAtExpand(this.nextFree.Length, itemSize);
                    var oldCount = this.nextFree.Length;
                    Dev.Assert(oldCount < need);
                    System.Array.Resize(ref this.nextFree, need);
                    MakeLink(oldCount);
                }

                // alloc
                var index = this.freeHead;
                this.freeHead = this.nextFree[index];

#if (FOR_DEBUG || UNITY_EDITOR) && !STORY_NO_DEBUG
                this.useCount++;
                this.worstCount = Mathf.Max(this.worstCount, this.useCount);
#endif

                return new(need, index);
            }

            /// <summary>Don't touch! Only for system.</summary>
            [MethodImpl(MethodImplOptions.NoInlining)] // !!! ジェネリクスによるコードブロート防止のため明示的にインライン化しない !!!
            internal bool TryExpand(int count)
            {
                Dev.Assert(0 < count);
                if (this.nextFree == null)
                {
                    this.freeHead = -1;
                    this.nextFree = new int[count];
                    MakeLink(0);
                }
                else
                {
                    var oldCount = this.nextFree.Length;

                    if (count <= oldCount)
                    {
                        Dev.LogWarning(string.Format(Messages.Warnings.ExpandOnly, oldCount, count));
                        return false;
                    }

                    System.Array.Resize(ref this.nextFree, count);
                    MakeLink(oldCount);
                }

                return true;
            }
        }

        // プールのスロット。
        // IDの有効性を検証するAgeを持つ。
        // 管理メタデータをデータと物理的に密着させることで、キャッシュ効率を向上させる。
        struct PoolSlot<T>
        {
            // fields

            public int Age; // 管理メタデータをデータと物理的に密着させる
            public T Value; // 実データ

            // methods

            /// <summary>Allocates a new identifier at the specified index and increments its generation age.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)] // サイズが小さい
            public Pool.Id Alloc(int index)
            {
                if (Age == 0) { Age = 1; }
                return new Pool.Id(index, Age);
            }

            /// <summary>Clears the slot value and advances its age to invalidate existing identifiers.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)] // サイズが小さい
            public void Free()
            {
                Value = default;
                Reborn();
            }

            /// <summary>Forcibly advances the generation age of this slot, skipping zero to ensure continuous validity.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)] // サイズが小さい
            public void Reborn() { if (++Age == 0) { Age = 1; } }
        }

        // 世代管理しないプール。indexの有効性を検証するためのオーバーヘッドがないぶん高速。
        // 何らかの手段でindexの有効性が保証されている場合に使用できるが、使用には細心の注意を払うこと！
        class UnsafePool<T> : IUnsafePool
#if (FOR_DEBUG || UNITY_EDITOR) && !STORY_NO_DEBUG
            , IPoolMonitorForDebug
#endif
        {
            // statics
            public static readonly UnsafePool<T> Shared = new();

            // fields
            PoolCore core;
            T[] array;

            // constructors

            protected UnsafePool() => Dev.PoolMonitorRegister(this); // 直接 new させない

            // methods

            /// <summary>Expands the unmanaged　pool capacity to the specified count if it exceeds the current capacity.</summary>
            public void Expand(int count)
            {
                if (this.core.TryExpand(count)) { Pool.Expand(ref this.array, count); }
            }

            /// <summary>Allocates a raw index from the unmanaged pool without age verification.</summary>
            // [MethodImpl(MethodImplOptions.AggressiveInlining)] // コンパイラに任せる（Alloc(in T)で呼ばれるので悩ましい）
            public int Alloc()
            {
                if (!this.core.TryAllocFast(out int index))
                {
                    var result = this.core.ExpandAndAlloc(Unsafe.SizeOf<T>());
                    if (0 < result.Need) { Pool.Expand(ref this.array, result.Need); }
                    index = result.Index;
                }
                return index;
            }

            /// <summary>Allocates a raw index and assigns the specified value without age verification.</summary>
            public int Alloc(in T value)
            {
                var ret = Alloc();
                this.array[ret] = value;
                return ret;
            }

            /// <summary>Releases the unmanaged slot at the specified index back to the pool.</summary>
            // [MethodImpl(MethodImplOptions.AggressiveInlining)] // 仮想メソッド
            public void Free(int index)
            {
                this.array[index] = default;
                this.core.Free(index);
            }

            /// <summary>Gets a reference to the unmanaged data at the raw index.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)] // サイズが小さい
            public ref T Get(int index) => ref this.array[index];

#if (FOR_DEBUG || UNITY_EDITOR) && !STORY_NO_DEBUG
            public string Name => Dev.Type<T>.Name;
            public int ActiveCount => this.core.ActiveCount;
            public int WorstCount => this.core.WorstCount;
            public int FreeCount => this.core.TotalCount - this.core.ActiveCount;
            public string PoolName => Dev.HiddenPool<T>.Name;
            public int TotalBytes => ArraySize + Unsafe.SizeOf<UnsafePool<T>>();
            protected int ArraySize => this.core.ArraySize + Unsafe.SizeOf<T>() * (this.array?.Length ?? 0);
#endif
        }
        interface IUnsafePool // 型消去用
        {
            void Free(int index);
#if (FOR_DEBUG || UNITY_EDITOR) && !STORY_NO_DEBUG
            string Name { get; }
#endif
        }

        // やめとこう...
//         public static ref T UnsafeGet<T>(this T[] array, int index) where T : struct
//         {
// #if NET_5_0_OR_GREATER || NET_COREAPP // 将来、Unityが CoreCLR（.NETモダンAPI）に完全移行した時のため
//             return ref Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(array), index);
// #else // iOS / Android / WebGL（.NET Standard 2.1 / IL2CPP環境）
//             // 参照が含まれている場合は、安全のために通常の配列アクセス
//             if (RuntimeHelpers.IsReferenceOrContainsReferences<T>()) { return ref array[index]; }

//             // 参照が含まれない純粋な値型配列なら、配列ヘッダを飛ばして直接アクセス
//             return ref Unsafe.Add(ref Unsafe.As<UnsafeDummy<T>>(array).Data, index);
//         }
//         class UnsafeDummy<T>
//         {
//             public IntPtr header;
//             public T Data;
// #endif
//         }

    }
}
