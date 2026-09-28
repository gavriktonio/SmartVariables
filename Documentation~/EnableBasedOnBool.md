# Enable Based on Bool

Add **Enable Based on Bool** to an active GameObject and assign a `BoolReference`. The controller applies the current value when enabled and follows subsequent changes. Its GameObject lists use `SetActive`, its component lists set `enabled`, and its BoolReference lists write the matching or inverse value.

The component lists accept any `Component` in the Inspector. `Behaviour` (including `MonoBehaviour`), `Renderer`, `Collider`, and `Collider2D` are supported. An unsupported component reports a Unity error when assigned or when the value is applied, and remains unchanged. Missing component entries are ignored.

Keep the controller outside any GameObject it disables, and do not put the controller itself in a component target list. Disabling the controller removes its BoolReference listener, so it cannot re-enable itself.