export function setCookie(name, value, expiresAt) {
    let expires = "";
    if(expiresAt) {
        expires = "; expires=" + expiresAt.toUTCString();
    }
    document.cookie = name + "=" + value + expires + "; path=/; Secure; SameSite=Lax";
}

export function launchBankId(autoStartToken, returnUrl) {
    const bankIdUrl = `bankid:///?autostarttoken=${autoStartToken}&redirect=${returnUrl}`;
    window.location.href = bankIdUrl;
}