import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, EventEmitter, Output, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { extractErrorMessage } from '../../core/http-error.util';
import { ProductService } from '../../core/product.service';
import { ToastService } from '../../shared/toast/toast.service';

@Component({
  selector: 'app-product-create',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './product-create.component.html',
  styleUrl: './product-create.component.scss'
})
export class ProductCreateComponent {
  private readonly fb = inject(FormBuilder);
  private readonly productService = inject(ProductService);
  private readonly toastService = inject(ToastService);

  @Output() created = new EventEmitter<void>();
  @Output() cancelled = new EventEmitter<void>();

  readonly isSubmitting = signal(false);

  readonly form = this.fb.nonNullable.group({
    name: ['', Validators.required],
    colour: ['', Validators.required],
    price: [0, [Validators.required, Validators.min(0)]]
  });

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isSubmitting.set(true);

    this.productService.create(this.form.getRawValue()).subscribe({
      next: () => {
        this.isSubmitting.set(false);
        this.toastService.success('Product created successfully.');
        this.created.emit();
      },
      error: (err: HttpErrorResponse) => {
        this.isSubmitting.set(false);
        this.toastService.error(extractErrorMessage(err, 'Failed to create product. Check the fields and try again.'));
      }
    });
  }
}
