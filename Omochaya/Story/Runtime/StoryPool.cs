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
            // プール（構造体配列）のId。
            // IndexとAgeを組み合わせて、IDの有効性を検証する。
            // Idの割り当てと解放はPool.Coreを使用して行われる。
            public readonly struct Id : IEquatable<Id>
            {
                // fields

                /// <summary>The internal array index and validation generation age of this identifier.</summary>
                public readonly int Index, Age;

                // constructors

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Id(int index, int age) { this.Index = index; this.Age = age; }

                // properties

                /// <summary>Gets a value indicating whether this identifier is valid and assigned.</summary>
                public readonly bool IsValid
                {
                    [MethodImpl(MethodImplOptions.AggressiveInlining)]
                    get => this.Age != 0;
                }

                // methods

                /// <summary>Determines whether this identifier matches the specified identifier.</summary>
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public readonly bool Matches(Id a) => this.Index == a.Index && this.Age == a.Age;

                // for collection（ユーザによる呼び出し禁止）

                /// <summary>Don't touch! Only for system.</summary>
                public bool Equals(Id other) => Matches(other);

                /// <summary>Don't touch! Only for system.</summary>
                public override bool Equals(object obj) => obj is Id other && Equals(other);

                /// <summary>Don't touch! Only for system.</summary>
                public override int GetHashCode() => HashCode.Combine(Index, Age);
            }

            /// <summary>Initializes a raw array using default capacity settings determined by item size calculations.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void Create<T>(ref T[] array)
                => CreateBasedOnItemSize(ref array, Unsafe.SizeOf<T>());

            /// <summary>Expands the capacity of a raw array based on default exponential scaling boundaries.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void Expand<T>(ref T[] array)
                => ExpandBasedOnItemSize(ref array, Unsafe.SizeOf<T>());

            /// <summary>Expands a raw array to a specific length, logging warning traces upon expansion profiles.</summary>
            public static void Expand<T>(ref T[] array, int length)
            {
                if (array == null) { array = new T[length]; }
                else
                {
                    Dev.Assert(array.Length < length);
                    Dev.SetInt(array.Length);
                    System.Array.Resize(ref array, length);
                    Dev.LogWarning(string.Format(Messages.Warnings.ArrayExpanded, Dev.GetInt(), array.Length, Dev.FormatMemorySize(Unsafe.SizeOf<T>() * array.Length), typeof(T).Name));
                }
            }

            /// <summary>Creates a raw array optimized within an initial memory limit threshold constraint.</summary>
            public static void CreateBasedOnItemSize<T>(ref T[] array, int itemSize, int limit = 1024 * 1)
            {
                var count = Mathf.Clamp(limit / itemSize, 8, 32);
                array = new T[count];
            }

            /// <summary>Expands a raw array scale with geometric clamping guidelines bounded by a maximum limit threshold.</summary>
            public static void ExpandBasedOnItemSize<T>(ref T[] array, int itemSize, int limit = 1024 * 128)
            {
                var newLength = array.Length;
                newLength += Mathf.Min(newLength, Mathf.Max(4, limit / itemSize));
                Dev.LogWarning(string.Format(Messages.Warnings.ArrayExpanded_BasedOn, array.Length, newLength, Dev.FormatMemorySize(Unsafe.SizeOf<T>() * newLength), typeof(T).Name));
                System.Array.Resize(ref array, newLength);
            }
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
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => this.pool != null;
            }

            // constructors

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            PoolMemory(IUnsafePool pool, int index) { this.pool = pool; this.index = index; }

            // creators

            /// <summary>Allocates a lightweight memory handle from the hidden global shared pool without generation tracking.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static PoolMemory Alloc<T>()
            {
                var pool = UnsafePool<T>.Shared;
                return new PoolMemory(pool, pool.Alloc());
            }

            /// <summary>Allocates a lightweight memory handle initialized with a value from the hidden global shared pool without generation tracking.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static PoolMemory Alloc<T>(in T value)
            {
                var pool = UnsafePool<T>.Shared;
                return new PoolMemory(pool, pool.Alloc(value));
            }

            // methods

            /// <summary>Explicitly releases the unmanaged memory slot handle back to its originating pool.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ref T Get<T>()
            {
                Dev.Assert(UnsafePool<T>.Shared == this.pool, string.Format("{0} != {1}. type is {2}", UnsafePool<T>.Shared, this.pool, typeof(T)));
                return ref UnsafePool<T>.Shared.Get(this.index);
            }

            /// <summary>Determines whether the allocated memory type does not match the specified type.</summary>
            // 型が一致しない
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool IsMissType<T>() => UnsafePool<T>.Shared != this.pool;

            /// <summary>Disposes of the unmanaged memory handle, safely recycling its index slot.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public void Expand(int length) => Pool.Expand(ref this.array, length);

                /// <summary>Allocates a new identifier from the underlying slot at the specified index.</summary>
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Pool.Id Alloc(int index) => this.array[index].Alloc(index);

                /// <summary>Allocates a new identifier from the underlying slot, assigning the initial value.</summary>
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Pool.Id Alloc(int index, in T value) => this.array[index].Alloc(index, in value);

                /// <summary>Frees the slot at the specified index and clears its value.</summary>
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public void Free(int index) => this.array[index].Free();

                /// <summary>Validates whether the given identifier matches the index and generation age of the active slot.</summary>
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public ref T UnsafeGet(int index)
                {
                    return ref this.array[index].Value;
                }

                /// <summary>Retrieves the current identifier for the raw index using its active generation age.</summary>
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Pool.Id UnsafeGetId(int index)
                {
                    return new Pool.Id(index, this.array[index].Age);
                }

                /// <summary>Forcibly updates the generation age of the slot matching the specified identifier.</summary>
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public void Reborn(Pool.Id id)
                {
                    Dev.Assert(IsValid(id));
                    this.array[id.Index].Reborn();
                }

                /// <summary>Forcibly updates the generation age of the slot at the raw index without validation.</summary>
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public void UnsafeReborn(int index) => this.array[index].Reborn();

#if (FOR_DEBUG || UNITY_EDITOR) && !STORY_NO_DEBUG
                public int ArraySize => Unsafe.SizeOf<PoolSlot<T>>() * (this.array?.Length ?? 0);
#endif
            }

            // fields
            PoolCore core;

            /// <summary>Don't touch! Only for system.</summary>
            protected PoolSlotArray array;

            // constructors
            protected PoolBase() => Dev.PoolMonitorRegister(this); // 直接 new させない

            // properties

            /// <summary>Gets the current length of the active core allocation pool.</summary>
            public int Length
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => this.core.Length;
            }

            // methods

            /// <summary>Allocates an empty slot from the pool and returns its unique identifier.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)] // 基本的にラップされて呼び出される（つまりT毎に1箇所からしか呼び出されない）のでここはインライン展開のほうがコードサイズ的にも有利になると判断。
            public Pool.Id Alloc()
            {
                if (this.core.IsUsedUp)
                {
                    this.core.ExpandBasedOnItemSizeAtAlloc(Unsafe.SizeOf<PoolSlot<T>>());
                    this.array.Expand(this.core.Length);
                }
                var index = this.core.Alloc();
                return this.array.Alloc(index);
            }

            /// <summary>Allocates a slot initialized with the specified value and returns its unique identifier.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)] // 基本的にラップされて呼び出される（つまりT毎に1箇所からしか呼び出されない）のでここはインライン展開のほうがコードサイズ的にも有利になると判断。
            public Pool.Id Alloc(in T value)
            {
                if (this.core.IsUsedUp)
                {
                    this.core.ExpandBasedOnItemSizeAtAlloc(Unsafe.SizeOf<PoolSlot<T>>());
                    this.array.Expand(this.core.Length);
                }
                var index = this.core.Alloc();
                return this.array.Alloc(index, in value);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)] // 基本的にラップされて呼び出される（つまりT毎に1箇所からしか呼び出されない）のでここはインライン展開のほうがコードサイズ的にも有利になると判断。
            protected int AllocCore<HOT>(ref HOT[] hotArray) // 派生クラス向けの処理...ここで書きたくない...
            {
                if (this.core.IsUsedUp)
                {
                    this.core.ExpandBasedOnItemSizeAtAlloc(Unsafe.SizeOf<PoolSlot<T>>() + Unsafe.SizeOf<HOT>());
                    this.array.Expand(this.core.Length);
                    Pool.Expand(ref hotArray, this.core.Length);
                }
                return this.core.Alloc();
            }

            /// <summary>Safely releases the slot associated with the specified identifier back to the pool.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Free(Pool.Id id)
            {
                if (!IsValid(id)) { return; }
                this.array.Free(id.Index);
                this.core.Free(id.Index);
            }

            /// <summary>Releases the slot at the raw index back to the pool without identifier validation.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void UnsafeFree(int index)
            {
                this.array.Free(index);
                this.core.Free(index);
            }

            /// <summary>Determines whether the specified identifier is valid within the underlying slot array.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ref T UnsafeGet(int index) => ref this.array.UnsafeGet(index);

            /// <summary>Retrieves the identifier corresponding to the raw index from the underlying slot array.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Pool.Id UnsafeGetId(int index) => this.array.UnsafeGetId(index);

            /// <summary>Advances the generation age of the slot associated with the specified identifier.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Reborn(Pool.Id id) => this.array.Reborn(id);

            /// <summary>Advances the generation age of the slot at the raw index without validation.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void UnsafeReborn(int index) => this.array.UnsafeReborn(index);

            /// <summary>Expands the pool capacity to the specified length if it exceeds the current capacity.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Expand(int length)
            {
                if (Length < length)
                {
                    this.core.ExpandArray(length);
                    this.array.Expand(length);
                }
                else { Dev.LogWarning(string.Format(Messages.Warnings.ExpandOnly, Length, length)); }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected void ExpandCore<HOT>(ref HOT[] hotArray, int length) // 派生クラス向けの処理...ここで書きたくない...けどパッと見で差分がわかり易くない？
            {
                if (Length < length)
                {
                    this.core.ExpandArray(length);
                    this.array.Expand(length);
                    Pool.Expand(ref hotArray, length);
                }
                else { Dev.LogWarning(string.Format(Messages.Warnings.ExpandOnly, Length, length)); }
            }

#if (FOR_DEBUG || UNITY_EDITOR) && !STORY_NO_DEBUG
            /// <summary>Don't touch! Only for system.</summary>
            public int ActiveCount => this.core.ActiveCount;
            /// <summary>Don't touch! Only for system.</summary>
            public int WorstCount { get; set; }
            /// <summary>Don't touch! Only for system.</summary>
            public int FreeCount => this.core.TotalCount - this.core.ActiveCount;
            /// <summary>Don't touch! Only for system.</summary>
            public abstract string PoolName { get; }
            /// <summary>Don't touch! Only for system.</summary>
            public abstract int TotalBytes { get; }
            /// <summary>Don't touch! Only for system.</summary>
            protected int ArraySize => this.array.ArraySize + this.core.ArraySize;
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

            /// <summary>Allocates a slot and returns its identifier, splitting management into hot and cool data arrays.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)] // 基本的にラップされて呼び出される（つまりT毎に1箇所からしか呼び出されない）のでここはインライン展開のほうがコードサイズ的にも有利になると判断。
            public new Pool.Id Alloc()
            {
                var index = AllocCore(ref this.hotArray);
                return this.array.Alloc(index);
            }

            /// <summary>Allocates a slot, initializes both its hot and cool elements, and returns its unique identifier.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)] // 基本的にラップされて呼び出される（つまりT毎に1箇所からしか呼び出されない）のでここはインライン展開のほうがコードサイズ的にも有利になると判断。
            public Pool.Id Alloc(in HOT hot, in COOL cool)
            {
                var index = AllocCore(ref this.hotArray);
                var ret = this.array.Alloc(index, in cool);
                this.hotArray[index] = hot;
                return ret;
            }

            /// <summary>Safely releases the slot and resets its associated hot and cool elements using the given identifier.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public new void Free(Pool.Id id)
            {
                if (!IsValid(id)) { return; }
                UnsafeFree(id.Index);
            }

            /// <summary>Releases the slot at the raw index without validation, resetting its hot and cool data.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ref COOL UnsafeGet2(int index) => ref base.UnsafeGet(index);

            /// <summary>Expands both the hot and cool data arrays to the specified length.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public new void Expand(int length) => ExpandCore(ref this.hotArray, length);

#if (FOR_DEBUG || UNITY_EDITOR) && !STORY_NO_DEBUG
            /// <summary>Don't touch! Only for system.</summary>
            protected new int ArraySize => base.ArraySize + Unsafe.SizeOf<COOL>() * (ActiveCount + FreeCount);
#endif
        }


        // 〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜
        // これ以降は間接的に使用されます。利用者が直接使用することは想定していません
        // 〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜

        // プールのコア機能。
        // 要素の確保と解放、ロック管理を担当する。
        // IDのうちIndexを管理し、必要に応じてプールの拡張も行う。
        struct PoolCore
        {
            // fields

            int[] nextFree;
            int freeHead;
            int useCount;

            // properties

            /// <summary>Gets the total capacity of the internal free-list array.</summary>
            public int Length
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => this.nextFree?.Length ?? 0;
            }

            /// <summary>Gets a value indicating whether all allocated slots have reached their usage maximum.</summary>
            public bool IsUsedUp
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => this.nextFree == null || this.freeHead == -1;
            }

            // methods

            // [MethodImpl(MethodImplOptions.AggressiveInlining)] // 呼び出し頻度が低いのでインライン化の判断はコンパイラに任せる
            void MakeLink(int oldLength)
            {
                var newLength = this.nextFree.Length;
                for (var i = oldLength; i < newLength-1; ++i) { this.nextFree[i] = i + 1; }
                this.nextFree[newLength - 1] = this.freeHead;
                this.freeHead = oldLength;
            }

            // [MethodImpl(MethodImplOptions.AggressiveInlining)] // 呼び出し頻度が低いのでインライン化の判断はコンパイラに任せる
            void CreateArray(int itemSize)
            {
                this.freeHead = -1;
                Pool.CreateBasedOnItemSize(ref this.nextFree, itemSize);
                MakeLink(0);
            }

            // [MethodImpl(MethodImplOptions.AggressiveInlining)] // 呼び出し頻度が低いのでインライン化の判断はコンパイラに任せる
            void ExpandArrayBasedOnItemSize(int itemSize)
            {
                var oldLength = this.nextFree.Length;
                Pool.ExpandBasedOnItemSize(ref this.nextFree, itemSize);
                MakeLink(oldLength);
            }

            /// <summary>Expands the free-list array to the specified capacity.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void ExpandArray(int newLength)
            {
                if (this.nextFree == null) { this.freeHead = -1; }
                var oldLength = Length;
                Pool.Expand(ref this.nextFree, newLength);
                MakeLink(oldLength);
            }

            /// <summary>Expands the free-list array automatically when an allocation occurs and capacity is exhausted.</summary>
            [MethodImpl(MethodImplOptions.NoInlining)] // ジェネリクスによるコードブロート防止のため明示的にインライン化しない
            public void ExpandBasedOnItemSizeAtAlloc(int itemSize)
            {
                if (this.nextFree == null) { CreateArray(itemSize); }
                else { ExpandArrayBasedOnItemSize(itemSize); }
            }

            /// <summary>Allocates a raw index from the free-list head.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Alloc()
            {
                var index = this.freeHead;
                this.freeHead = this.nextFree[index];
                this.useCount++;
                return index;
            }

            /// <summary>Returns the specified index back to the free-list.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Free(int index)
            {
                this.nextFree[index] = this.freeHead;
                this.freeHead = index;
                this.useCount--;
            }

#if (FOR_DEBUG || UNITY_EDITOR) && !STORY_NO_DEBUG
            public readonly int ActiveCount => this.useCount;
            public readonly int TotalCount => this.nextFree?.Length ?? 0;
            public readonly int ArraySize => Unsafe.SizeOf<int>() * TotalCount;
#endif
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
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Pool.Id Alloc(int index)
            {
                if (Age == 0) { Age = 1; }
                return new Pool.Id(index, Age);
            }

            /// <summary>Allocates a new identifier at the specified index, initializes its value, and increments its age.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Pool.Id Alloc(int index, in T value)
            {
                if (Age == 0) { Age = 1; }
                Value = value;
                return new Pool.Id(index, Age);
            }

            /// <summary>Clears the slot value and advances its age to invalidate existing identifiers.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Free()
            {
                Value = default;
                Reborn();
            }

            /// <summary>Forcibly advances the generation age of this slot, skipping zero to ensure continuous validity.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

            /// <summary>Allocates a raw index from the unmanaged pool without age verification.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)] // 基本的にラップされて呼び出される（つまりT毎に1箇所からしか呼び出されない）のでここはインライン展開のほうがコードサイズ的にも有利になると判断。
            public int Alloc()
            {
                if (this.core.IsUsedUp)
                {
                    this.core.ExpandBasedOnItemSizeAtAlloc(Unsafe.SizeOf<T>());
                    Pool.Expand(ref this.array, this.core.Length);
                }
                return this.core.Alloc();
            }

            /// <summary>Allocates a raw index and assigns the specified value without age verification.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)] // 基本的にラップされて呼び出される（つまりT毎に1箇所からしか呼び出されない）のでここはインライン展開のほうがコードサイズ的にも有利になると判断。
            public int Alloc(in T value)
            {
                var ret = Alloc();
                this.array[ret] = value;
                return ret;
            }

            /// <summary>Releases the unmanaged slot at the specified index back to the pool.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Free(int index)
            {
                this.array[index] = default;
                this.core.Free(index);
            }

            /// <summary>Gets a reference to the unmanaged data at the raw index.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ref T Get(int index) => ref this.array[index];

#if (FOR_DEBUG || UNITY_EDITOR) && !STORY_NO_DEBUG
            public string Name => Dev.Type<T>.Name;
            public int ActiveCount => this.core.ActiveCount;
            public int WorstCount { get; set; }
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
//         // [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
