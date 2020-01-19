using System;
using System.Collections.Generic;

namespace ARMeilleure.Translation
{
    class JitCacheMemoryAllocator
    {
        private int _size;

        private LinkedList<(int Start, int End)> _memoryRanges;

        public JitCacheMemoryAllocator(int size, int startPosition)
        {
            _size = size;

            _memoryRanges = new LinkedList<(int start, int end)>();

            _memoryRanges.AddFirst((startPosition, startPosition - 1));
        }

        public int Allocate(int size)
        {
            var node = _memoryRanges.First;

            int offset;

            while (true)
            {
                if (node.Value.End > (_size - 1) - size)
                {
                    throw new OutOfMemoryException();
                }

                if (node.Next == null)
                {
                    offset = node.Value.End + 1;

                    node.Value = (node.Value.Start, node.Value.End + size);

                    break;
                }
                else
                {
                    if (node.Next.Value.Start - node.Value.End <= 1)
                    {
                        node.Value = (node.Value.Start, node.Next.Value.End);

                        _memoryRanges.Remove(node.Next);
                    }

                    if (node.Next.Value.Start - size > node.Value.End)
                    {
                        offset = node.Value.End + 1;

                        if (node.Next.Value.Start - offset == size)
                        {
                            node.Value = (node.Value.Start, node.Next.Value.End);

                            _memoryRanges.Remove(node.Next);

                            break;
                        }

                        node.Value = (node.Value.Start, offset + size - 1);

                        break;
                    }

                    node = node.Next;
                }
            }

            return offset;
        }

        public void Free(int offset, int size)
        {
            if ((uint)offset >= (ulong)_size)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }

            var node = _memoryRanges.First;

            while (true)
            {
                if (node == null)
                {
                    throw new ArgumentOutOfRangeException(nameof(offset));
                }

                if (offset <= node.Value.End)
                {
                    int newRangeStart = offset + size;

                    _memoryRanges.AddAfter(node, (newRangeStart, node.Value.End));

                    break;
                }

                node = node.Next;
            }

            node.Value = (node.Value.Start, offset - 1);
        }
    }
}
