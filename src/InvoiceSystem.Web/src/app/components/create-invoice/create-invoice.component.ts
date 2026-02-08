import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { InvoiceService } from '../../services/invoice.service';
import { CreateInvoiceRequest } from '../../models/invoice.model';

@Component({
  selector: 'app-create-invoice',
  templateUrl: './create-invoice.component.html',
  styleUrls: ['./create-invoice.component.css']
})
export class CreateInvoiceComponent {
  invoice: CreateInvoiceRequest = {
    customer: {
      name: '',
      email: '',
      phone: '',
      address: ''
    },
    issueDate: new Date(),
    dueDate: new Date(Date.now() + 30 * 24 * 60 * 60 * 1000), // 30 days from now
    items: [
      {
        description: '',
        quantity: 1,
        unitPrice: 0
      }
    ]
  };

  loading = false;
  error: string | null = null;

  constructor(
    private invoiceService: InvoiceService,
    private router: Router
  ) { }

  addItem(): void {
    this.invoice.items.push({
      description: '',
      quantity: 1,
      unitPrice: 0
    });
  }

  removeItem(index: number): void {
    if (this.invoice.items.length > 1) {
      this.invoice.items.splice(index, 1);
    }
  }

  calculateItemTotal(item: any): number {
    return item.quantity * item.unitPrice;
  }

  calculateTotal(): number {
    return this.invoice.items.reduce((total, item) => total + this.calculateItemTotal(item), 0);
  }

  onSubmit(): void {
    this.loading = true;
    this.error = null;

    if (!this.validateDueDate()) {
      this.error = 'Due date must be greater than today\'s date.';
      this.loading = false;
      return;
    }

    this.invoiceService.createInvoice(this.invoice).subscribe({
      next: (response) => {
        this.loading = false;
        this.router.navigate(['/invoices']);
      },
      error: (error) => {
        console.error('Error creating invoice:', error);
        this.error = 'Failed to create invoice. Please check your input and try again.';
        this.loading = false;
      }
    });
  }

  onCancel(): void {
    this.router.navigate(['/invoices']);
  }

  onIssueDateChange(event: any): void {
    this.invoice.issueDate = new Date(event.target.value);
  }

  onDueDateChange(event: any): void {
    this.invoice.dueDate = new Date(event.target.value);
    this.validateDueDate();
  }

  validateDueDate(): boolean {
    const today = new Date();
    const dueDate = new Date(this.invoice.dueDate);
    today.setHours(0, 0, 0, 0);
    dueDate.setHours(0, 0, 0, 0);
    return dueDate > today;
  }

  isDueDateValid(): boolean {
    return this.validateDueDate();
  }

  getTomorrowDate(): string {
    const tomorrow = new Date();
    tomorrow.setDate(tomorrow.getDate() + 1);
    return tomorrow.toISOString().split('T')[0];
  }
}