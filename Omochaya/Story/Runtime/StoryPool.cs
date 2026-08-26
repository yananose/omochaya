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
        /// <summary>Provides global configurations, identifier structures, and capacity calculation utilities for the unified pooling system.</summary>
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
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public bool Equals(Id other) => Matches(other);

                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public override bool Equals(object obj) => obj is Id other && Equals(other);

                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public override int GetHashCode() => HashCode.Combine(Index, Age);
            }

            internal const int CREATE_LIMIT_SIZE = 1024 * 1;

            internal const int EXPAND_LIMIT_SIZE = 1024 * 128;

            /// <summary>Expands the specified raw array to the exact capacity, logging a diagnostic warning upon resizing.</summary>
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

#if (STORY_DEBUG || UNITY_EDITOR) && !STORY_NO_DEBUG
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
        public class Pool<T> : PoolBase<PoolMeta, T>
        {
            /// <summary>The globally shared singleton instance of this single-type pool.</summary>
            public static readonly Pool<T> Shared = new();
            Pool() { }

#if (STORY_DEBUG || UNITY_EDITOR) && !STORY_NO_DEBUG
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public override string PoolName => Dev.Pool<T>.Name;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public override int TotalBytes => ItemSize * Length + Unsafe.SizeOf<Pool<T>>();
#endif
        }

        /// <summary>A split-data instance pool managing hot and cool data structures for enhanced cache efficiency.</summary>
        public class Pool<HOT, COOL> : PoolBase<PoolMeta, HOT, COOL>
        {
            /// <summary>The globally shared singleton instance of this split hot/cool data pool.</summary>
            public static readonly Pool<HOT, COOL> Shared = new();
            Pool() { }

#if (STORY_DEBUG || UNITY_EDITOR) && !STORY_NO_DEBUG
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public override string PoolName => Dev.Pool<HOT, COOL>.Name;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public override int TotalBytes => ItemSize * Length + Unsafe.SizeOf<Pool<HOT, COOL>>();
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
            // inner classes

            struct UnsafePoolMeta : IUnsafePoolMeta { }

            class HiddenPool<T> : UnsafePoolBase<UnsafePoolMeta, T>
            {
                internal static readonly HiddenPool<T> Shared = new();
                HiddenPool() {}

#if (STORY_DEBUG || UNITY_EDITOR) && !STORY_NO_DEBUG
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public override string PoolName => Dev.HiddenPool<T>.Name;
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public override int TotalBytes => ItemSize * Length + Unsafe.SizeOf<HiddenPool<T>>();
#endif
            }

            // fields

            readonly PoolCore pool;
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
            PoolMemory(PoolCore pool, int index) { this.pool = pool; this.index = index; }

            // methods

            /// <summary>Expands the underlying unmanaged pool capacity for the specified type to the exact count.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void Expand<T>(int count)
                => HiddenPool<T>.Shared.Expand(count);

            /// <summary>Allocates a lightweight memory handle from the hidden global shared pool without generation tracking.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static PoolMemory Alloc<T>()
            {
                var pool = HiddenPool<T>.Shared;
                return new PoolMemory(pool, pool.Alloc());
            }

            /// <summary>Allocates a lightweight memory handle initialized with a value from the hidden global shared pool without generation tracking.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static PoolMemory Alloc<T>(in T value)
            {
                var pool = HiddenPool<T>.Shared;
                var ret = new PoolMemory(pool, pool.Alloc());
                pool.UnsafeGet(ret.index) = value;
                return ret;
            }

            /// <summary>Explicitly releases the unmanaged memory slot handle back to its originating pool.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Free()
            {
                if (IsValid) { this.pool.UnsafeFree(this.index); }
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
                Dev.RuntimeAssert(HiddenPool<T>.Shared == this.pool, string.Format("{0} != {1}. type is {2}", HiddenPool<T>.Shared, this.pool, typeof(T)));
                return ref HiddenPool<T>.Shared.UnsafeGet(this.index);
            }

            /// <summary>Determines whether the allocated memory type does not match the specified type.</summary>
            // 型が一致しない
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool IsMissType<T>() => HiddenPool<T>.Shared != this.pool;

            /// <summary>Disposes of the unmanaged memory handle, safely recycling its index slot.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Dispose() => Free();
        }

        // 型によっては使わないジェネリッククラスのメソッドを拡張メソッドとすることでコードブロートを抑制

        /// <summary>Gets a direct reference to the metadata at the raw index without identifier validation.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref M UnsafeGetMeta<M, V>(this UnsafePoolBase<M, V> self, int index)
            where M : struct, IUnsafePoolMeta
            => ref self.UnsafeGetSlot(index).Meta;

        /// <summary>Gets a reference to the metadata associated with the specified valid identifier.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref M GetMeta<M, V>(this PoolBase<M, V> self, Pool.Id id)
            where M : struct, IPoolMeta
        {
            Dev.RuntimeAssert(self.IsValid(id));
            return ref self.UnsafeGetMeta(id.Index);
            
        }

        /// <summary>Forcibly advances the generation age of the metadata at the raw index without validation.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UnsafeReborn<M, V>(this PoolBase<M, V> self, int index)
            where M : struct, IPoolMeta
        {
            ref var meta = ref self.UnsafeGetMeta(index);
            var age = meta.Age;
            if (++age == 0) { age = 1; }
            meta.Age = age;
        }

        /// <summary>Advances the generation age of the metadata associated with the specified identifier.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Reborn<M, V>(this PoolBase<M, V> self, Pool.Id id)
            where M : struct, IPoolMeta
        {
            if (self.IsValid(id)) { self.UnsafeReborn(id.Index); }
        }

        /// <summary>Retrieves the current identifier for the raw index using its active generation age.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Pool.Id UnsafeGetId<M, V>(this PoolBase<M, V> self, int index)
            where M : struct, IPoolMeta
            => new Pool.Id(index, self.UnsafeGetMeta(index).Age);
    }

// }

// 〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜
// これ以降は間接的に使用されます。利用者が直接使用することは想定していません
// 〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜

// namespace Omochaya.HiddenStory
// {
//     using System;
//     using System.Runtime.CompilerServices;
//     using UnityEngine;

    /// <summary>Don't touch! Only for system.（継承しないでください）</summary>
    public interface IUnsafePoolMeta { }

    /// <summary>Don't touch! Only for system.（継承しないでください）</summary>
    public interface IPoolMeta : IUnsafePoolMeta { int Age { get; set; } }

    /// <summary>Don't touch! Only for system.</summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public struct PoolMeta : IPoolMeta
    {
        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public int Age { get; set; }
    }

    /// <summary>Don't touch! Only for system.（継承しないでください）</summary>
    public abstract class PoolCore
#if (STORY_DEBUG || UNITY_EDITOR) && !STORY_NO_DEBUG
        : IPoolMonitorForDebug
#endif
    {
        // fields
        int[] nextFree;
        int count = 1;
        int freeHead;
        int createLimitSize = Story.Pool.CREATE_LIMIT_SIZE;
        int expandLimitSize = Story.Pool.EXPAND_LIMIT_SIZE;

#if (STORY_DEBUG || UNITY_EDITOR) && !STORY_NO_DEBUG
        int useCount;
        int worstCount;

        // properties

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public int ActiveCount => this.useCount;
        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public int WorstCount => this.worstCount;
        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public int FreeCount => Length - ActiveCount;
        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public abstract string PoolName { get; }
        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public abstract int TotalBytes { get; }
#endif

        /// <summary>Gets a value indicating whether the internal pool structures have been initialized and are valid.</summary>
        public bool IsValid
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this.nextFree != null;
        }

        /// <summary>Gets the current total allocated capacity of the underlying pool array.</summary>
        public int Length
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this.count;
        }

        protected int ItemSize
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Unsafe.SizeOf<int>();
        }

        // constructors

        // methods

        protected abstract int ExpandArray(int count);

        [MethodImpl(MethodImplOptions.NoInlining)] // 派生クラスから呼ばれるのでコードブロート防止
        protected int GetNeedCount(int itemSize)
            => this.nextFree == null ?
                Story.Pool.GetNeedCountAtCreate(itemSize, this.createLimitSize):
                Story.Pool.GetNeedCountAtExpand(this.nextFree.Length, itemSize, this.expandLimitSize);

        /// <summary>Expands the pool capacity to the specified count if it exceeds the current capacity.</summary>
        // [MethodImpl(MethodImplOptions.AggressiveInlining)] // コンパイラに任せる
        public void Expand(int newCount)
        {
            var oldCount = Length;
            if (0 < newCount && newCount <= oldCount)
            {
                Dev.LogWarning(string.Format(Messages.Warnings.ExpandOnly, oldCount, newCount));
                return;
            }
            if (this.nextFree == null) { this.freeHead = -1; Dev.PoolMonitorRegister(this); }
            newCount = ExpandArray(newCount);
            Story.Pool.Expand(ref this.nextFree, newCount);
            this.count = newCount;
            for (var i = oldCount; i < newCount-1; ++i) { this.nextFree[i] = i + 1; }
            this.nextFree[newCount - 1] = this.freeHead;
            this.freeHead = oldCount;
        }

        // [MethodImpl(MethodImplOptions.AggressiveInlining)] // コンパイラに任せる
        internal int Alloc()
        {
            if (this.freeHead == -1) { Expand(0); }

            var index = this.freeHead;
            this.freeHead = this.nextFree == null ? -1 : this.nextFree[index];

#if (STORY_DEBUG || UNITY_EDITOR) && !STORY_NO_DEBUG
            this.useCount++;
            this.worstCount = Mathf.Max(this.worstCount, this.useCount);
#endif

            return index;
        }

        /// <summary>Releases the slot at the raw index back to the pool without identifier validation.</summary>
        // [MethodImpl(MethodImplOptions.AggressiveInlining)] // コンパイラに任せる
        public virtual void UnsafeFree(int index)
        {
            if (this.nextFree == null) { Dev.Assert(index == 0); }
            else { this.nextFree[index] = this.freeHead; }
            this.freeHead = index;

#if (STORY_DEBUG || UNITY_EDITOR) && !STORY_NO_DEBUG
            this.useCount--;
#endif
        }

        /// <summary>Configures the memory limit thresholds for initial creation and subsequent expansions of the pool capacity.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Custom(int createLimitSize = Story.Pool.CREATE_LIMIT_SIZE, int expandLimitSize = Story.Pool.EXPAND_LIMIT_SIZE)
        {
            this.createLimitSize = createLimitSize;
            this.expandLimitSize = expandLimitSize;
        }
    }

// ↓↓↓↓↓↓ ここからジェネリクスによるコードブロート対象 ↓↓↓↓↓↓

    /// <summary>Don't touch! Only for system.（継承しないでください）</summary>
    public abstract class UnsafePoolBase<M, V> : PoolCore
        where M : struct, IUnsafePoolMeta
    {
        // inner classes
        internal struct PoolSlot
        {
            internal M Meta;
            internal V Value;
        }

        // fields
        PoolSlot[] array;
        PoolSlot origin;

        // properties

        protected new int ItemSize
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => base.ItemSize + Unsafe.SizeOf<PoolSlot>();
        }

        // methods

        // [MethodImpl(MethodImplOptions.AggressiveInlining)] // コンパイラに任せる
        protected override int ExpandArray(int count)
        {
            if (count <= 0) { count = GetNeedCount(ItemSize); }
            var copy = this.array == null;
            Story.Pool.Expand(ref this.array, count);
            if (copy) { this.array[0] = this.origin; }
            return count;
        }

        /// <summary>Releases the slot at the raw index back to the pool without identifier validation.</summary>
        // [MethodImpl(MethodImplOptions.AggressiveInlining)] // コンパイラに任せる
        public override void UnsafeFree(int index)
        {
            base.UnsafeFree(index);
            if (this.array == null) { this.origin.Value = default; }
            else { this.array[index].Value = default; }
        }

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
        public ref V UnsafeGet(int index) => ref UnsafeGetSlot(index).Value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)] // サイズが小さい
        internal ref PoolSlot UnsafeGetSlot(int index)
        {
            if (this.array == null) { return ref this.origin; }
            else { return ref this.array[index]; }
        }
    }

    /// <summary>Don't touch! Only for system.（継承しないでください）</summary>
    public abstract class PoolBase<M, V> : UnsafePoolBase<M, V>
        where M : struct, IPoolMeta
    {
        /// <summary>Allocates an empty slot from the pool and returns its unique identifier.</summary>
        // [MethodImpl(MethodImplOptions.AggressiveInlining)] // コンパイラに任せる
        public new Story.Pool.Id Alloc()
        {
            var index = base.Alloc();
            ref var meta = ref this.UnsafeGetSlot(index).Meta;
            if (meta.Age == 0) { meta.Age = 1; }
            return new Story.Pool.Id(index, meta.Age);
        }

        /// <summary>Safely releases the slot associated with the specified identifier back to the pool.</summary>
        // [MethodImpl(MethodImplOptions.AggressiveInlining)] // コンパイラに任せる
        public void Free(Story.Pool.Id id)
        {
            if (IsValid(id)) { UnsafeFree(id.Index); }
        }

        /// <summary>Releases the slot at the raw index back to the pool without identifier validation.</summary>
        // [MethodImpl(MethodImplOptions.AggressiveInlining)] // コンパイラに任せる
        public override void UnsafeFree(int index)
        {
            base.UnsafeFree(index);
            ref var meta = ref this.UnsafeGetSlot(index).Meta;
            var age = meta.Age;
            if (++age == 0) { age = 1; }
            meta.Age = age;
        }

        /// <summary>Determines whether the specified identifier is valid within the underlying slot array.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] // サイズが小さい
        public new bool IsValid(Story.Pool.Id id)
            => (uint)id.Index < this.Length && this.UnsafeGetSlot(id.Index).Meta.Age == id.Age;

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
        public ref V Get(Story.Pool.Id id)
        {
            Dev.RuntimeAssert(IsValid(id));
            return ref UnsafeGet(id.Index);
        }

#if (STORY_DEBUG || UNITY_EDITOR) && !STORY_NO_DEBUG
        [Obsolete("このメソッドは廃止されました。代わりに Get(Alloc()) = value を使用してください。", true)]
        public Story.Pool.Id Alloc(in V value)
            => throw new NotSupportedException();
#endif
    }

    /// <summary>Don't touch! Only for system.（継承しないでください）</summary>
    // 2つの型を管理するプールの基底クラス。PoolBase<T>を継承し、追加のデータ配列を管理する。
    // hot なデータを分離することでキャッシュ効率の向上を狙う。
    public abstract class PoolBase<M, HOT, COOL> : PoolBase<M, COOL>
        where M : struct, IPoolMeta
    {
        // fields
        HOT[] array;
        HOT origin;

        // properties
        protected new int ItemSize
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => base.ItemSize + Unsafe.SizeOf<HOT>();
        }

        // methods

        // [MethodImpl(MethodImplOptions.AggressiveInlining)] // コンパイラに任せる
        protected override int ExpandArray(int count)
        {
            count = base.ExpandArray(count);
            var copy = this.array == null;
            Story.Pool.Expand(ref this.array, count);
            if (copy) { this.array[0] = this.origin; }
            return count;
        }

        /// <summary>Releases the slot at the raw index without validation, resetting its hot and cool data.</summary>
        // [MethodImpl(MethodImplOptions.AggressiveInlining)] // コンパイラに任せる
        public override void UnsafeFree(int index)
        {
            base.UnsafeFree(index);
            if (this.array == null) { this.origin = default; }
            else { this.array[index] = default; }
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
        public new ref HOT Get(Story.Pool.Id id)
        {
            Dev.RuntimeAssert(IsValid(id));
            return ref UnsafeGet(id.Index);
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
        public new ref HOT UnsafeGet(int index)
        {
            if (this.array == null) { return ref this.origin; }
            else { return ref this.array[index]; }
        }

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
        public ref COOL Get2(Story.Pool.Id id) => ref base.Get(id);

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

#if (STORY_DEBUG || UNITY_EDITOR) && !STORY_NO_DEBUG
        [Obsolete("このメソッドは廃止されました。代わりに var id = Alloc(); Get(id) = hot; Get2(id) = cool; を使用してください。", true)]
        public Story.Pool.Id Alloc(in HOT hot, in COOL cool)
            => throw new NotSupportedException();
#endif
    }

// ↑↑↑↑↑↑ ここまでジェネリクスによるコードブロート対象 ↑↑↑↑↑↑

}
