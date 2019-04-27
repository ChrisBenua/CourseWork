using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace M138ADemo.MainClasses
{
    /// <summary>
    /// Data grid extensions.
    /// </summary>
    public static class DataGridExtensions
    {
        /// <summary>
        /// Finds the visual parent of type T.
        /// </summary>
        /// <returns>The visual parent.</returns>
        /// <param name="element">Element.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T FindVisualParent<T>(Visual element) where T : Visual
        {
            Visual parent = element;
            while (parent != null)
            {
                T correctlyTyped = parent as T;
                if (correctlyTyped != null)
                {
                    return correctlyTyped;
                }
                parent = VisualTreeHelper.GetParent(parent) as Visual;
            }
            return null;
        }

        /// <summary>
        /// Gets the cell.
        /// </summary>
        /// <returns>The cell.</returns>
        /// <param name="mygrid">Mygrid.</param>
        /// <param name="row">Row.</param>
        /// <param name="column">Column.</param>
        public static DataGridCell GetCell(this DataGrid mygrid, int row, int column)
        {
            var rowContainer = mygrid.GetRow(row);

            if (rowContainer != null)
            {
                var presenter = GetVisualChild<DataGridCellsPresenter>(rowContainer);
                if (presenter == null)
                    return null;

                // try to get the cell but it may possibly be virtualized
                var cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                if (cell == null)
                {
                    // now try to bring into view and retreive the cell
                    mygrid.ScrollIntoView(rowContainer, mygrid.Columns[column]);
                    cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                }
                return cell;
            }
            return null;
        }

        /// <summary>
        /// Gets the row by index.
        /// </summary>
        /// <returns>The row.</returns>
        /// <param name="host">Host datagrid.</param>
        /// <param name="index">Index of row.</param>
        public static DataGridRow GetRow(this DataGrid host, int index)
        {
            var row = (DataGridRow)host.ItemContainerGenerator.ContainerFromIndex(index);
            if (row == null)
            {
                // may be virtualized, bring into view and try again
                host.ScrollIntoView(host.Items[index]);
                row = (DataGridRow)host.ItemContainerGenerator.ContainerFromIndex(index);
            }
            return row;
        }

        /// <summary>
        /// Gets the visual child of type T.
        /// </summary>
        /// <returns>The visual child.</returns>
        /// <param name="parent">Parent.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T GetVisualChild<T>(Visual parent) where T : Visual
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                var v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T ?? GetVisualChild<T>(v);
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }

    }
}
