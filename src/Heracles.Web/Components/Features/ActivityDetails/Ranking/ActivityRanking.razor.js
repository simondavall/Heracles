export function makeDraggable(panel) {
    const dragHandle = panel.querySelector("[data-drag-handle]");
    const resizeHandle = panel.querySelector("[data-resize-handle]");

    let dragging = false;
    let dragPointerId = null;
    let dragOffsetX = 0;
    let dragOffsetY = 0;

    let resizing = false;
    let resizePointerId = null;
    let resizeStartX = 0;
    let resizeStartY = 0;
    let resizeStartWidth = 0;
    let resizeStartHeight = 0;

    const onDragPointerDown = event => {
        if (event.button !== 0)
            return;

        if (event.target.closest("button"))
            return;

        const rect = panel.getBoundingClientRect();

        dragging = true;
        dragPointerId = event.pointerId;
        dragOffsetX = event.clientX - rect.left;
        dragOffsetY = event.clientY - rect.top;

        panel.style.left = `${rect.left}px`;
        panel.style.top = `${rect.top}px`;
        panel.style.transform = "none";

        dragHandle.setPointerCapture(dragPointerId);
        event.preventDefault();
    };

    const onDragPointerMove = event => {
        if (!dragging || event.pointerId !== dragPointerId)
            return;

        const maxLeft = Math.max(0, window.innerWidth - panel.offsetWidth);
        const maxTop = Math.max(0, window.innerHeight - panel.offsetHeight);

        const left = clamp(event.clientX - dragOffsetX, 0, maxLeft);
        const top = clamp(event.clientY - dragOffsetY, 0, maxTop);

        panel.style.left = `${left}px`;
        panel.style.top = `${top}px`;
    };

    const onDragPointerUp = event => {
        if (!dragging || event.pointerId !== dragPointerId)
            return;

        dragging = false;

        if (dragHandle.hasPointerCapture(dragPointerId))
            dragHandle.releasePointerCapture(dragPointerId);

        dragPointerId = null;
    };

    const onResizePointerDown = event => {
        if (event.button !== 0)
            return;

        const rect = panel.getBoundingClientRect();

        resizing = true;
        resizePointerId = event.pointerId;
        resizeStartX = event.clientX;
        resizeStartY = event.clientY;
        resizeStartWidth = rect.width;
        resizeStartHeight = rect.height;

        panel.style.left = `${rect.left}px`;
        panel.style.top = `${rect.top}px`;
        panel.style.width = `${rect.width}px`;
        panel.style.height = `${rect.height}px`;
        panel.style.transform = "none";

        panel.classList.add("resized");

        resizeHandle.setPointerCapture(resizePointerId);
        event.preventDefault();
        event.stopPropagation();
    };

    const onResizePointerMove = event => {
        if (!resizing || event.pointerId !== resizePointerId)
            return;

        const rect = panel.getBoundingClientRect();

        const availableWidth = window.innerWidth - rect.left;
        const availableHeight = window.innerHeight - rect.top;

        const minimumWidth = getMinimumWidth(panel);
        const minimumHeight = getMinimumHeight(panel);

        const width = clamp(resizeStartWidth + (event.clientX - resizeStartX), minimumWidth, availableWidth);
        const height = clamp(resizeStartHeight + (event.clientY - resizeStartY), minimumHeight, availableHeight);

        panel.style.width = `${width}px`;
        panel.style.height = `${height}px`;
    };

    const onResizePointerUp = event => {
        if (!resizing || event.pointerId !== resizePointerId)
            return;

        resizing = false;

        if (resizeHandle.hasPointerCapture(resizePointerId))
            resizeHandle.releasePointerCapture(resizePointerId);

        resizePointerId = null;
    };

    const onWindowResize = () => {
        const rect = panel.getBoundingClientRect();

        const width = Math.min(rect.width, window.innerWidth);
        const height = Math.min(rect.height, window.innerHeight);
        const left = clamp(rect.left, 0, Math.max(0, window.innerWidth - width));
        const top = clamp(rect.top, 0, Math.max(0, window.innerHeight - height));

        panel.style.left = `${left}px`;
        panel.style.top = `${top}px`;

        if (panel.classList.contains("resized")) {
            panel.style.width = `${width}px`;
            panel.style.height = `${height}px`;
        }

        panel.style.transform = "none";
    };

    if (dragHandle) {
        dragHandle.addEventListener("pointerdown", onDragPointerDown);
        dragHandle.addEventListener("pointermove", onDragPointerMove);
        dragHandle.addEventListener("pointerup", onDragPointerUp);
        dragHandle.addEventListener("pointercancel", onDragPointerUp);
    }

    if (resizeHandle) {
        resizeHandle.addEventListener("pointerdown", onResizePointerDown);
        resizeHandle.addEventListener("pointermove", onResizePointerMove);
        resizeHandle.addEventListener("pointerup", onResizePointerUp);
        resizeHandle.addEventListener("pointercancel", onResizePointerUp);
    }

    window.addEventListener("resize", onWindowResize);

    return {
        dispose() {
            if (dragHandle) {
                dragHandle.removeEventListener("pointerdown", onDragPointerDown);
                dragHandle.removeEventListener("pointermove", onDragPointerMove);
                dragHandle.removeEventListener("pointerup", onDragPointerUp);
                dragHandle.removeEventListener("pointercancel", onDragPointerUp);
            }

            if (resizeHandle) {
                resizeHandle.removeEventListener("pointerdown", onResizePointerDown);
                resizeHandle.removeEventListener("pointermove", onResizePointerMove);
                resizeHandle.removeEventListener("pointerup", onResizePointerUp);
                resizeHandle.removeEventListener("pointercancel", onResizePointerUp);
            }

            window.removeEventListener("resize", onWindowResize);
        }
    };
}

export function centreCurrentActivity(panel) {
    const list = panel.querySelector("[data-ranking-list]");
    const currentActivity = panel.querySelector('[data-current-activity="true"]');

    if (!list || !currentActivity)
        return;

    const targetScrollTop = currentActivity.offsetTop - ((list.clientHeight - currentActivity.offsetHeight) / 2);
    const maximumScrollTop = list.scrollHeight - list.clientHeight;

    list.scrollTop = clamp(targetScrollTop, 0, Math.max(0, maximumScrollTop));
}

function getMinimumWidth(panel) {
    const minimumWidth = parseFloat(getComputedStyle(panel).minWidth);
    return Number.isFinite(minimumWidth) ? minimumWidth : 0;
}

function getMinimumHeight(panel) {
    const minimumHeight = parseFloat(getComputedStyle(panel).minHeight);
    return Number.isFinite(minimumHeight) ? minimumHeight : 0;
}

function clamp(value, minimum, maximum) {
    return Math.min(Math.max(value, minimum), maximum);
}