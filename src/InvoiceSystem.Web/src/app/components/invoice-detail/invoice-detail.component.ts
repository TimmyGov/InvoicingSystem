import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Invoice } from '../../models/invoice.model';
import { InvoiceService } from '../../services/invoice.service';

@Component({
  selector: 'app-invoice-detail',
  templateUrl: './invoice-detail.component.html',
  styleUrls: ['./invoice-detail.component.css']
})
export class InvoiceDetailComponent implements OnInit {
  invoice: Invoice | null = null;
  loading = false;
  error: string | null = null;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private invoiceService: InvoiceService
  ) { }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.loadInvoice(id);
    } else {
      this.error = 'Invalid invoice ID';
    }
  }

  loadInvoice(id: string): void {
    this.loading = true;
    this.error = null;
    
    this.invoiceService.getInvoiceById(id).subscribe({
      next: (data) => {
        this.invoice = data;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading invoice:', error);
        this.error = 'Failed to load invoice details. Please try again later.';
        this.loading = false;
      }
    });
  }

  getStatusClass(status: any): string {
    switch (status) {
      case 0: // Draft
        return 'badge bg-secondary';
      case 1: // Sent
        return 'badge bg-primary';
      case 2: // Paid
        return 'badge bg-success';
      case 3: // Overdue
        return 'badge bg-danger';
      case 4: // Cancelled
        return 'badge bg-warning';
      default:
        return 'badge bg-secondary';
    }
  }

  getStatusText(status: any): string {
    switch (status) {
      case 0:
        return 'Draft';
      case 1:
        return 'Sent';
      case 2:
        return 'Paid';
      case 3:
        return 'Overdue';
      case 4:
        return 'Cancelled';
      default:
        return 'Unknown';
    }
  }

  formatDate(date: Date): string {
    return new Date(date).toLocaleDateString('en-ZA');
  }

  formatCurrency(amount: number): string {
    return new Intl.NumberFormat('en-ZA', {
      style: 'currency',
      currency: 'ZAR'
    }).format(amount);
  }

  calculateItemTotal(item: any): number {
    return item.quantity * item.unitPrice;
  }

  goBack(): void {
    this.router.navigate(['/invoices']);
  }

  editInvoice(): void {
    if (this.invoice) {
      this.router.navigate(['/invoices/edit', this.invoice.id]);
    }
  }

  printInvoice(): void {
    window.print();
  }
}