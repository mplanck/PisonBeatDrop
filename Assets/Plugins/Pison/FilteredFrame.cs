/* Pison Filtered Frames
 * Class for receiving filtered frames from the device for finger lift.
 */

using System.Collections.Generic;

public class FilteredFrame
{
    public string filterName;
    public List<int> channels;

    public FilteredFrame(string name, List<int> channels)
    {
        this.filterName = name;
        this.channels = channels;
    }
}