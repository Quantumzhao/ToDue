# ToDue

## What is it?

It's basically a todo list. 

Originally I planned to implement it as a *Rainmeter* widget, but failed for all sorts of reasons. 

## What are the unique features?

- [x] It is simple and concise, more of a resemblance of the classical sticky-note-on-monitor solution
- [x] It pins on desktop just like a *Windows 7* gadget. Great for reminding people (especially me) that they have works to do, but without the hassle of constantly buying sticky notes and make the monitor look messy
- [x] It also shows the due dates, color coded in a helpful way
- [x] You can pin items and reorganize them with total freedom (given that the "auto reordering" option is turned off)
- [x] You can highlight the even more important todo items

## Screenshots

### Initial design

<img src="design.jpg" alt="design" style="zoom:50%;" />

### Actual:

<img src=".\new.png" alt="new" style="zoom:67%;" />

## Other

- It automatically create a shortcut in `Startup` folder to make it automatically start on *Windows* start. You can always disable it in the task manager. 
- It is intentionally hidden from the "Application" category in the task manager, so that <kbd>Win</kbd> + <kbd>Tab</kbd> won't show it
- The font `segmdl2` is copyrighted by *Microsoft*
- The font `CONSERVATIVE SIMPLICITY` is created by myself. It is not allowed for commercial purposes. For more copyright info, please visit my [*Behance* page](https://www.behance.net/gallery/49352569/Font-Design-Conservative-Simplicity)

---

## Checklist

- [x] Transparent background
- [x] Title
- [x] Todo item data structure
- [x] List view of items
    - [x] Layout
        - [x] Adaptive
        - [x] Meet design style
    - [x] Data binding
        - [x] *Remove* button
        - [x] Item name
        - [x] Data
    - [x] Style
        - [x] Visual Style
            - [x] *Remove* button
            - [x] Item name
            - [x] Data
- [x] Add button
    - [x] Application logic
        - [x] Building domain model from inputs
        - [x] Adding & Sorting
    - [x] Style
        - [x] Visual Style
    - [x] Layout
- [x] Separator
- [x] Draggable interface
- [x] Keep on desktop
- [x] Show in notification area
    - [x] Setting transparency
    - [x] Switching between light/dark mode
    - [x] Exit
    - [x] Reset position
- [x] An icon
- [x] Read from/write to file
- [x] Embed fonts
- [x] Start on bootup