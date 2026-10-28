// Potvrzovací dialog pro smazání (Bootstrap modal uprostřed stránky).
// Použití ve formuláři: <form ... onsubmit="return confirmDelete(this)" data-name="Rex">
// Volitelně data-title="Smazat druh?" změní nadpis dialogu (výchozí "Smazat zvíře?").
// Funkce vrací false, aby se formulář hned neodeslal; odešle se až po kliknutí na "Smazat".
let deleteDialog = null;
let formToSubmit = null;

function confirmDelete(form) {
    if (!deleteDialog) {
        deleteDialog = createDeleteDialog();
    }

    formToSubmit = form;
    deleteDialog.element.querySelector('.js-dialog-title').textContent = form.dataset.title || 'Smazat zvíře?';
    deleteDialog.element.querySelector('.js-dialog-item-name').textContent = form.dataset.name || 'tuto položku';
    deleteDialog.modal.show();

    return false;
}

function createDeleteDialog() {
    const element = document.createElement('div');
    element.className = 'modal fade delete-dialog';
    element.tabIndex = -1;
    element.setAttribute('aria-labelledby', 'delete-dialog-title');
    element.setAttribute('aria-hidden', 'true');
    element.innerHTML = `
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content border-0 shadow-lg rounded-4">
                <div class="modal-body text-center p-4 pt-5">
                    <div class="delete-dialog-icon mx-auto mb-3">
                        <svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" fill="currentColor" viewBox="0 0 16 16" aria-hidden="true">
                            <path d="M5.5 5.5A.5.5 0 0 1 6 6v6a.5.5 0 0 1-1 0V6a.5.5 0 0 1 .5-.5m2.5 0a.5.5 0 0 1 .5.5v6a.5.5 0 0 1-1 0V6a.5.5 0 0 1 .5-.5m3 .5a.5.5 0 0 0-1 0v6a.5.5 0 0 0 1 0z"/>
                            <path d="M14.5 3a1 1 0 0 1-1 1H13v9a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V4h-.5a1 1 0 0 1-1-1V2a1 1 0 0 1 1-1H6a1 1 0 0 1 1-1h2a1 1 0 0 1 1 1h3.5a1 1 0 0 1 1 1zM4.118 4 4 4.059V13a1 1 0 0 0 1 1h6a1 1 0 0 0 1-1V4.059L11.882 4zM2.5 3h11V2h-11z"/>
                        </svg>
                    </div>
                    <h2 class="h4 mb-2 js-dialog-title" id="delete-dialog-title"></h2>
                    <p class="text-body-secondary mb-0">
                        Opravdu si přejete smazat <strong class="js-dialog-item-name text-body"></strong>?<br />
                        Tuto akci nelze vrátit zpět.
                    </p>
                </div>
                <div class="modal-footer border-0 justify-content-center gap-2 pb-4">
                    <button type="button" class="btn btn-outline-secondary px-4" data-bs-dismiss="modal">Zrušit</button>
                    <button type="button" class="btn btn-danger px-4 js-dialog-confirm">Smazat</button>
                </div>
            </div>
        </div>`;

    document.body.appendChild(element);

    element.querySelector('.js-dialog-confirm').addEventListener('click', function () {
        if (formToSubmit) {
            // form.submit() neprovede znovu onsubmit, takže se dialog neotevře podruhé.
            formToSubmit.submit();
        }
    });

    element.addEventListener('hidden.bs.modal', function () {
        formToSubmit = null;
    });

    return { element: element, modal: new bootstrap.Modal(element) };
}
