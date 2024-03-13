export function createMap(coordinates, zoom) 
{
    const map = L.map('map').setView(coordinates ?? [51.505, -0.09], zoom ?? 13);
    window.map1 = map;

    L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
    }).addTo(map);

    return map;
}

export function addMarker(map, coordinates, text) 
{
    const marker = L.marker(coordinates ?? [51.5, -0.09]).addTo(map);

    if (text)
    {
        marker
            .bindPopup(text);
        //.openPopup();
    }
}