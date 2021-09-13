# ToDue

## What is it?

It's basically a todo list. 

Originally I planned to implement it as a *Rainmeter* widget, but failed for all sorts of reasons. 

## What are the unique features?

- It is simple and concise, more of a resemblance of the classical sticky-note-on-monitor
- It pins on desktop just like a *Windows 7* gadget. Great for reminding people (especially me) that they have works to do
- It also shows the due dates

## Screenshots

### Initial design

<img src="design.jpg" alt="design" style="zoom:50%;" />

### Actual:

<img src="C:\Users\yisha\source\repos\ToDue\new.png" alt="new" style="zoom:33%;" />

### Actual (Legacy):

<img src="actual.jpg" alt="actual" style="zoom:50%;" />

## Other

- Create a shortcut in `Startup` folder to make it automatically start on *Windows* start. 

- The font `segmdl2` is copyrighted by *Microsoft*

- The font `CONSERVATIVE SIMPLICITY` is created by myself. It is not allowed for commercial purposes. For more copyright info, please visit my *Behance* page

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
- [ ] Show in notification area
    - [x] Setting transparency
    - [ ] Switching between light/dark mode
    - [x] Exit
    - [x] Reset position
- [x] An icon
- [x] Read from/write to file
- [x] Embed fonts
- [x] Start on bootup