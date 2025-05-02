class Layout {
    static initialize() {
        this.bindChatToggleEvents();
    }

    static bindChatToggleEvents() {
        const chatToggleBtn = document.getElementById('chatToggleBtn');
        const chatCloseBtn = document.getElementById('chatCloseBtn');
        const chatPanel = document.getElementById('chatPanel');
        const chatOverlay = document.getElementById('chatOverlay');

        // --- Chat Panel Events ---
        if (chatToggleBtn) {
            chatToggleBtn.addEventListener('click', () => {
                chatPanel?.classList.add('chat-open');
                chatOverlay?.classList.add('active');
            });
        }

        if (chatCloseBtn) {
            chatCloseBtn.addEventListener('click', () => {
                chatPanel?.classList.remove('chat-open');
                chatOverlay?.classList.remove('active');
            });
        }

        // --- Overlay Event ---
        if (chatOverlay) {
            chatOverlay.addEventListener('click', () => {
                // Close chat panel when overlay is clicked
                chatPanel?.classList.remove('chat-open');
                chatOverlay?.classList.remove('active');
            });
        }

        // Optional: Close chat on escape key press
        document.addEventListener('keydown', (event) => {
            if (event.key === 'Escape' && chatPanel?.classList.contains('chat-open')) {
                chatPanel.classList.remove('chat-open');
                chatOverlay?.classList.remove('active');
            }
        });
    }

    // Removed openPanel, closePanel, showOverlay, hideOverlay helper methods as they are simple toggles now
}