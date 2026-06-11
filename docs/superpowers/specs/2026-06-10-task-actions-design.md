# Issue #30: Task Actions (Delete & Status Update) — Design Doc

## Component Tree

```
TasksView.vue
 ├── TaskCard.vue (extracted from inline card)
 │    └── TaskActions.vue (Edit, Delete buttons + Status dropdown)
 ├── DeleteConfirmDialog.vue
 └── TaskFormModal.vue (existing)
```

## Data Flow

| Emit | Source | Handler (TasksView) |
|---|---|---|
| `edit(task)` | TaskActions → TaskCard | Opens TaskFormModal in edit mode |
| `delete(task)` | TaskActions → TaskCard | Sets deletingTask, opens DeleteConfirmDialog |
| `status-change({task, newStatus})` | TaskActions → TaskCard | Calls store.updateTask with optimistic update |
| `confirm` | DeleteConfirmDialog | Calls store.deleteTask with optimistic removal, toast |
| `cancel` | DeleteConfirmDialog | Closes dialog |

## Component: TaskCard.vue

**Props:** `task: Task`, `showActions: boolean`

**States:** default, hover (desktop shows actions on `group-hover:`)

Extracts the existing inline `<button>` card from TasksView.vue into a reusable component. Wraps in `<div>` instead of `<button>`, clicking the card body emits `edit`. Contains `TaskActions` positioned at bottom-right.

## Component: TaskActions.vue

**Props:** `task: Task`, `loading: boolean`

**Emits:** `edit`, `delete`, `status-change(task, newStatus)`

- **Status dropdown:** `<select>` with Pending/InProgress/Completed, badge-colored, emits `status-change` on change
- **Edit button:** Pencil icon, emits `edit`
- **Delete button:** Trash icon (red), emits `delete`
- **Loading state:** All actions disabled with spinner during API calls
- **Desktop:** Actions hidden by default, shown on card hover
- **Mobile:** Always visible

## Component: DeleteConfirmDialog.vue

**Props:** `isOpen: boolean`, `taskTitle: string`, `loading: boolean`

**Emits:** `confirm`, `cancel`

- Matches TaskFormModal's styling (backdrop, dialog, X button, ESC close)
- Shows: "Are you sure you want to delete '{taskTitle}'? This action cannot be undone."
- Cancel button (secondary) + Delete button (danger red with spinner)
- Disabled state during loading, focus management

## Store Changes (tasksStore.ts)

Both actions keep their existing signatures; internals become optimistic:

- **`deleteTask(id)`**: snapshot `tasks.value` → optimistic remove → `await deleteTaskApi(id)` → on error, restore snapshot + throw
- **`updateTask(id, dto)`**: snapshot old task → optimistic update → `await updateTaskApi(id, dto)` → on error, revert + throw

## Toast Notifications

**Install:** `vue-toastification@next`

**Register** in `main.ts` with 3-second timeout.

**Toasts fired from TasksView:**
- `toast.success("Task deleted successfully")` on delete success
- `toast.error("Failed to delete task")` on delete error
- `toast.success("Status updated successfully")` on status update success
- `toast.error("Failed to update status")` on status update error

## Files to Create

```
apps/web/src/components/tasks/TaskCard.vue
apps/web/src/components/tasks/TaskActions.vue
apps/web/src/components/tasks/DeleteConfirmDialog.vue
apps/web/src/components/tasks/__tests__/TaskCard.spec.ts
apps/web/src/components/tasks/__tests__/TaskActions.spec.ts
apps/web/src/components/tasks/__tests__/DeleteConfirmDialog.spec.ts
```

## Files to Modify

```
apps/web/src/stores/tasksStore.ts      — optimistic UI in deleteTask + updateTask
apps/web/src/views/TasksView.vue       — integrate TaskCard, handle emits, toasts
apps/web/src/main.ts                   — register vue-toastification
```

## Acceptance Criteria

1. Edit button opens TaskFormModal in edit mode with pre-filled data
2. Delete button shows confirmation dialog with task title
3. Cancel closes dialog, Confirm calls API with optimistic removal
4. Optimistic UI: task removed immediately, restored on error
5. Status dropdown updates optimistically, reverts on error
6. Toast notifications for all success/error outcomes
7. Loading spinners on buttons during API calls
8. Works on mobile (always visible) and desktop (hover to show)
9. All new tests pass
10. Code follows existing Vue 3 Composition API + Tailwind patterns
