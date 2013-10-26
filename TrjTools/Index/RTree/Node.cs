/**
 * <p>Used by RTree. There are no public methods in this class.</p>
 * 
 * @author aled@sourceforge.net
 * @modified by liyaguang0123@gmail.com
 * @version 1.0b2p1
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrjTools.Index.RTree
{
    public class Node<T>
    {
        internal int nodeId = 0;
        internal Rectangle mbr = null;
        internal Rectangle[] entries = null;
        internal int[] ids = null;
        internal int level;
        internal int entryCount;

        public Node(int nodeId, int level, int maxNodeEntries)
        {
            this.nodeId = nodeId;
            this.level = level;
            entries = new Rectangle[maxNodeEntries];
            ids = new int[maxNodeEntries];
        }

        internal void addEntry(Rectangle r, int id)
        {
            ids[entryCount] = id;
            entries[entryCount] = r.copy();
            entryCount++;
            if (mbr == null)
            {
                mbr = r.copy();
            }
            else
            {
                mbr.add(r);
            }
        }

        internal void addEntryNoCopy(Rectangle r, int id)
        {
            ids[entryCount] = id;
            entries[entryCount] = r;
            entryCount++;
            if (mbr == null)
            {
                mbr = r.copy();
            }
            else
            {
                mbr.add(r);
            }
        }

        // Return the index of the found entry, or -1 if not found
        internal int findEntry(Rectangle r, int id)
        {
            for (int i = 0; i < entryCount; i++)
            {
                if (id == ids[i] && r.Equals(entries[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        // delete entry. This is done by setting it to null and copying the last entry into its space.
        internal void deleteEntry(int i, int minNodeEntries)
        {
            int lastIndex = entryCount - 1;
            Rectangle deletedRectangle = entries[i];
            entries[i] = null;
            if (i != lastIndex)
            {
                entries[i] = entries[lastIndex];
                ids[i] = ids[lastIndex];
                entries[lastIndex] = null;
            }
            entryCount--;

            // if there are at least minNodeEntries, adjust the MBR.
            // otherwise, don't bother, as the Node<T> will be 
            // eliminated anyway.
            if (entryCount >= minNodeEntries)
            {
                recalculateMBR(deletedRectangle);
            }
        }

        // oldRectangle is a rectangle that has just been deleted or made smaller.
        // Thus, the MBR is only recalculated if the OldRectangle influenced the old MBR
        internal void recalculateMBR(Rectangle deletedRectangle)
        {
            if (mbr.edgeOverlaps(deletedRectangle))
            {
                mbr.set(entries[0].min, entries[0].max);

                for (int i = 1; i < entryCount; i++)
                {
                    mbr.add(entries[i]);
                }
            }
        }

        public int getEntryCount()
        {
            return entryCount;
        }

        public Rectangle getEntry(int index)
        {
            if (index < entryCount)
            {
                return entries[index];
            }
            return null;
        }

        public int getId(int index)
        {
            if (index < entryCount)
            {
                return ids[index];
            }
            return -1;
        }

        /**
         * eliminate null entries, move all entries to the start of the source node
         */
        internal void reorganize(RTree<T> rtree)
        {
            int countdownIndex = rtree.maxNodeEntries - 1;
            for (int index = 0; index < entryCount; index++)
            {
                if (entries[index] == null)
                {
                    while (entries[countdownIndex] == null && countdownIndex > index)
                    {
                        countdownIndex--;
                    }
                    entries[index] = entries[countdownIndex];
                    ids[index] = ids[countdownIndex];
                    entries[countdownIndex] = null;
                }
            }
        }

        internal bool isLeaf()
        {
            return (level == 1);
        }

        public int getLevel()
        {
            return level;
        }

        public Rectangle getMBR()
        {
            return mbr;
        }
    }
}

