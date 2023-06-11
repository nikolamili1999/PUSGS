/**
 * Cuva podatke o proizvodu u okviru porudzbine
 * Skladisti podatke o proizvodu(id, naziv, cenu)
 * kao i podatke o kolicini proizvoda i ukupnoj ceni (cena * kolicina)
 */
export class OrderItem {
    id: number;
    productId: number;
    orderId: number;
    quantity: number;
    productPrice: number;
    productName: string;
    price: number;
}