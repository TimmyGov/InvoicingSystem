export enum InvoiceStatus {
  Draft = 0,
  Sent = 1,
  Paid = 2,
  Overdue = 3,
  Cancelled = 4
}

export interface Customer {
  id: string;
  name: string;
  email: string;
  phone?: string;
  address?: string;
}

export interface InvoiceItem {
  id: string;
  description: string;
  quantity: number;
  unitPrice: number;
  amount: number;
}

export interface Invoice {
  id: string;
  invoiceNumber: string;
  issueDate: Date;
  dueDate: Date;
  status: InvoiceStatus;
  totalAmount: number;
  createdAt: Date;
  updatedAt?: Date;
  customer: Customer;
  items: InvoiceItem[];
}

export interface CreateInvoiceItem {
  description: string;
  quantity: number;
  unitPrice: number;
}

export interface CreateCustomer {
  name: string;
  email: string;
  phone?: string;
  address?: string;
}

export interface CreateInvoiceRequest {
  customer: CreateCustomer;
  issueDate: Date;
  dueDate: Date;
  items: CreateInvoiceItem[];
}
