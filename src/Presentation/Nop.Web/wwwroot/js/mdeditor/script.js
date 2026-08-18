const logger = console;
/**
 * Initializes the Markdown editor
 * @param {string} textareaId - ID of the <textarea> element.
 * @param {string} iconsPath - Path to the icons folder (must end with '/').
 */
function initializeMarkdownEditor(textareaId, iconsPath) {
logger.debug(`>>> Entering initializeMarkdownEditor(textareaId=${textareaId},iconsPath=${iconsPath})`);
  const markdownInput = document.getElementById(textareaId);
  const htmlOutput = document.getElementById('html-output');
logger.debug(`initializeMarkdownEditor(textareaId=${textareaId},iconsPath=${iconsPath}): markdownInput → ${markdownInput}`);
logger.debug(`initializeMarkdownEditor(textareaId=${textareaId},iconsPath=${iconsPath}): htmlOutput → ${htmlOutput}`);
  const toolbar = document.getElementById('editor-toolbar');
  const editorContainer = markdownInput.closest('.markdown-editor-container');
logger.debug(`initializeMarkdownEditor(textareaId=${textareaId},iconsPath=${iconsPath}): toolbar → ${toolbar}`);
logger.debug(`initializeMarkdownEditor(textareaId=${textareaId},iconsPath=${iconsPath}): editorContainer → ${editorContainer}`);

  if (!markdownInput || !htmlOutput || !toolbar || !editorContainer) {
    console.error("Error: Required DOM elements not found (textarea, output, toolbar, or container).");
    return;
  }

  toolbar.style.display = 'flex';

  // Add scroll synchronization
  const writePanel = document.getElementById('write-panel');
logger.debug(`initializeMarkdownEditor(textareaId=${textareaId},iconsPath=${iconsPath}): writePanel → ${writePanel}`);
  const previewPanel = document.getElementById('preview-panel');
logger.debug(`initializeMarkdownEditor(textareaId=${textareaId},iconsPath=${iconsPath}): previewPanel → ${previewPanel}`);

  if (writePanel && previewPanel) {
    markdownInput.addEventListener('scroll', () => {
      const ratio = markdownInput.scrollTop / (markdownInput.scrollHeight - markdownInput.clientHeight);
      previewPanel.scrollTop = ratio * (previewPanel.scrollHeight - previewPanel.clientHeight);
    });
  }

  // Save the editor height in localStorage
  function saveEditorHeight() {
	logger.debug('>>> Entering saveEditorHeight()');
    localStorage.setItem('markdownEditorHeight', editorContainer.style.height);
	logger.debug('<<< Exiting saveEditorHeight()');
  }

  // Restore the editor height from localStorage
  const savedHeight = localStorage.getItem('markdownEditorHeight');
  if (savedHeight) {
    editorContainer.style.height = savedHeight;
  }

  // Add resize observer to save height when resized
  const resizeObserver = new ResizeObserver(() => {
    saveEditorHeight();
  });
  resizeObserver.observe(editorContainer);

  // Preview update function
  function updatePreview() {
	logger.debug('>>> Entering updatePreview()');
    const markdownText = markdownInput.value;
    // Use marked.parse() for conversion
    // Add GFM (GitHub Flavored Markdown) option for better compatibility
    // and breaks option to treat newlines as <br>
    htmlOutput.innerHTML = marked.parse(markdownText, { gfm: true, breaks: true });
	logger.debug('<<< Exiting updatePreview()');
  }

  // Helper to insert text into textarea
  /**
   * Inserts markup into the textarea around selected text or at the cursor position.
   * @param {string} prefix - Text to insert before the selection/cursor.
   * @param {string} [suffix=''] - Text to insert after the selection/cursor.
   * @param {string} [placeholder='text'] - Placeholder text if nothing is selected.
   */
  function insertMarkdown(prefix, suffix = '', placeholder = 'text') {
	logger.debug(`>>> Entering insertMarkdown(prefix=${prefix},suffix=${suffix},placeholder=${placeholder})`);
    const start = markdownInput.selectionStart;
    const end = markdownInput.selectionEnd;
    const selectedText = markdownInput.value.substring(start, end);
	logger.debug(`insertMarkdown(prefix=${prefix},suffix=${suffix},placeholder=${placeholder}): selectedText → ${selectedText}`);
    const textToInsert = selectedText || placeholder;

    const before = markdownInput.value.substring(0, start);
	logger.debug(`insertMarkdown(prefix=${prefix},suffix=${suffix},placeholder=${placeholder}): before → ${before}`);
    const after = markdownInput.value.substring(end);
	logger.debug(`insertMarkdown(prefix=${prefix},suffix=${suffix},placeholder=${placeholder}): after → ${after}`);

    markdownInput.value = before + prefix + textToInsert + suffix + after;

    // Set cursor position
    if (selectedText) {
      // If text was selected, select the inserted text
      markdownInput.selectionStart = start + prefix.length;
      markdownInput.selectionEnd = markdownInput.selectionStart + textToInsert.length;
    } else {
      // If no text was selected, place cursor inside the inserted placeholder
      markdownInput.selectionStart = start + prefix.length;
      markdownInput.selectionEnd = markdownInput.selectionStart + placeholder.length;
    }

    markdownInput.focus();
    updatePreview();
	logger.debug(`<<< Exiting insertMarkdown(prefix=${prefix},suffix=${suffix},placeholder=${placeholder})`);
  }

  // Helper to insert block-level elements like images
  /**
   * Inserts block-level Markdown, potentially adding newlines before/after.
   * @param {string} markdownToInsert - The complete Markdown string to insert.
   */
  function insertBlockMarkdown(markdownToInsert) {
	logger.debug(`>>> Entering insertBlockMarkdown(markdownToInsert=${markdownToInsert})`);
    const start = markdownInput.selectionStart;
    const end = markdownInput.selectionEnd;
    const before = markdownInput.value.substring(0, start);
	logger.debug(`insertBlockMarkdown(markdownToInsert=${markdownToInsert}): before → ${before}`);
    const after = markdownInput.value.substring(end);
	logger.debug(`insertBlockMarkdown(markdownToInsert=${markdownToInsert}): after → ${after}`);

    // Add newline before/after if inserting mid-line and no newline exists
    const prefix = (start > 0 && markdownInput.value[start - 1] !== '\n') ? '\n' : '';
	logger.debug(`insertBlockMarkdown(markdownToInsert=${markdownToInsert}): prefix → ${prefix}`);
    const suffix = (end < markdownInput.value.length && markdownInput.value[end] !== '\n') ? '\n' : '';
	logger.debug(`insertBlockMarkdown(markdownToInsert=${markdownToInsert}): suffix → ${suffix}`);

    markdownInput.value = before + prefix + markdownToInsert + suffix + after;

    // Set cursor position after the inserted text
    const cursorPos = start + prefix.length + markdownToInsert.length;
    markdownInput.selectionStart = cursorPos;
    markdownInput.selectionEnd = cursorPos;

    markdownInput.focus();
    updatePreview();
	logger.debug(`<<< Exiting insertBlockMarkdown(markdownToInsert=${markdownToInsert})`);
  }

  // Toolbar button definitions
  const toolbarButtons = [
    { title: 'Heading', icon: 'heading.svg', action: () => insertMarkdown('## ', '', 'Heading') },
    { title: 'Bold', icon: 'bold.svg', action: () => insertMarkdown('**', '**', 'bold text') },
    { title: 'Italic', icon: 'italic.svg', action: () => insertMarkdown('*', '*', 'italic text') },
    { title: 'Blockquote', icon: 'quote.svg', action: () => insertMarkdown('> ', '', 'quote') },
    { title: 'Code (Inline)', icon: 'code.svg', action: () => insertMarkdown('`', '`', 'code') },
    { title: 'Code (Block)', icon: 'code-block.svg', action: () => insertMarkdown('```\n', '\n```', 'code') },
    {
      title: 'Link', icon: 'link.svg', action: () => {
        const url = prompt('Enter link URL:', 'http://');
        if (url) {
          insertMarkdown('[', `](${url})`, 'link text');
        }
      }
    },
    {
      title: 'Image', icon: 'image.svg', action: () => {
        const url = prompt('Enter image URL:', 'http://');
        if (!url) return;

        const altText = prompt('Enter image description (alt text):', 'description');
        insertBlockMarkdown(`![${altText === null ? '' : altText}](${url})`);
      }
    },
    { title: 'Bulleted List', icon: 'list-ul.svg', action: () => insertMarkdown('* ', '', 'list item') },
    { title: 'Numbered List', icon: 'list-ol.svg', action: () => insertMarkdown('1. ', '', 'list item') },
  ];

  // Create toolbar buttons
  toolbar.innerHTML = '';
  toolbarButtons.forEach(buttonConfig => {
    const button = document.createElement('button');
    button.type = 'button'; 
    button.title = buttonConfig.title; 

    const img = document.createElement('img');
    img.src = iconsPath + buttonConfig.icon;
    img.alt = buttonConfig.title;
    // Handle icon loading errors
    img.onerror = () => { console.warn(`Failed to load icon: ${img.src}`); img.alt = `[${buttonConfig.title}]`; };

    button.appendChild(img);
    button.addEventListener('click', buttonConfig.action);
    toolbar.appendChild(button);
  });

  // Add 'input' event listener to the textarea
  // Update preview on input, even if the Preview tab is not active
  markdownInput.addEventListener('input', updatePreview);

  // Update preview on initialization
  updatePreview();

  console.log("Markdown editor initialized."); 

  // Return the update function reference for potential use by tabs (though not strictly needed by current setupTabs)
logger.debug(`<<< Exiting initializeMarkdownEditor(textareaId=${textareaId},iconsPath=${iconsPath})`);
  return { updatePreview };
}

function clickTab(button, tabButtons, tabPanels, markdownInput) {
logger.debug(`>>> Entering clickTab(button=${button},tabButtons=${tabButtons},tabPanels=${tabPanels},markdownInput=${markdownInput})`);
  const targetTab = button.dataset.tab;
  const toolbar = document.getElementById('editor-toolbar');
logger.debug(`clickTab(button=${button},tabButtons=${tabButtons},tabPanels=${tabPanels},markdownInput=${markdownInput}): toolbar → ${toolbar}`);

  // 1. Update buttons
  tabButtons.forEach(btn => btn.classList.remove('active'));
  button.classList.add('active');

  // 2. Update panels (without destroying content)
  tabPanels.forEach(panel => {
    if (panel.id === `${targetTab}-panel`) {
      panel.classList.add('active');
    } else {
      panel.classList.remove('active');
    }
  });

  // 3. Update preview if switching to the Preview tab
  if (targetTab === 'preview') {
    toolbar.style.display = 'none';
  } else if (targetTab === 'write') {
    toolbar.style.display = 'flex';
	logger.debug(`<<< Exiting clickTab(button=${button},tabButtons=${tabButtons},tabPanels=${tabPanels},markdownInput=${markdownInput})`);
  }
}

/**
 * Sets up the tab switching logic.
 * @param {string} containerId - ID of the main editor container.
 * @param {string} textareaId - ID of the <textarea> element.
 */
function setupTabs(containerId, textareaId) {
logger.debug(`>>> Entering setupTabs(containerId=${containerId},textareaId=${textareaId})`);
  const container = document.getElementById(containerId);
  if (!container) {
    console.error(`Error: Tab container with ID '${containerId}' not found.`);
    return;
  }

  const markdownInput = document.getElementById(textareaId);
  const tabButtons = container.querySelectorAll('.editor-tabs .tab-button');
  const tabPanels = container.querySelectorAll('.editor-content .tab-panel');

  tabButtons.forEach(button => {
    button.addEventListener('click', (e) => {
      // Prevent the default form submission
      e.preventDefault();
      e.stopPropagation();

      clickTab(button, tabButtons, tabPanels, markdownInput);
    });
  });

  // Set focus to input field on initial load if Write tab is active
  const writePanel = document.getElementById('write-panel');
  if (writePanel && writePanel.classList.contains('active')) {
    if (markdownInput) markdownInput.focus();
  }
logger.debug(`<<< Exiting setupTabs(containerId=${containerId},textareaId=${textareaId})`);
}
