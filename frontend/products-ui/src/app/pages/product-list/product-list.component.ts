import { CommonModule } from '@angular/common';
import { Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/auth.service';
import { HealthService } from '../../core/health.service';
import { ProductService } from '../../core/product.service';
import { Product } from '../../models/product.model';
import { ProductCreateComponent } from '../product-create/product-create.component';

const PAGE_SIZE = 5;

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, ProductCreateComponent],
  templateUrl: './product-list.component.html',
  styleUrl: './product-list.component.scss'
})
export class ProductListComponent implements OnInit {
  readonly products = signal<Product[]>([]);
  readonly isLoading = signal(false);
  readonly errorMessage = signal<string | null>(null);
  readonly page = signal(1);
  readonly totalPages = signal(1);
  readonly totalCount = signal(0);

  readonly isHealthy = signal<boolean | null>(null);
  readonly uptimeLabel = signal('');

  readonly showCreateModal = signal(false);

  colourFilter = '';
  minPriceFilter: number | null = null;
  maxPriceFilter: number | null = null;

  constructor(
    private readonly productService: ProductService,
    private readonly authService: AuthService,
    private readonly healthService: HealthService,
    private readonly router: Router
  ) {}

  ngOnInit(): void {
    this.load();
    this.checkHealth();
  }

  private checkHealth(): void {
    this.healthService.check().subscribe({
      next: (health) => {
        this.isHealthy.set(health.status.toLowerCase() === 'healthy');
        // Backend returns a TimeSpan string like "00:03:41.1234567" — trim to whole seconds.
        this.uptimeLabel.set(health.uptime.split('.')[0]);
      },
      error: () => this.isHealthy.set(false)
    });
  }

  search(): void {
    this.page.set(1);
    this.load();
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages()) {
      return;
    }
    this.page.set(page);
    this.load();
  }

  private load(): void {
    this.isLoading.set(true);
    this.errorMessage.set(null);

    this.productService
      .getAll({
        colour: this.colourFilter.trim() || undefined,
        minPrice: this.minPriceFilter ?? undefined,
        maxPrice: this.maxPriceFilter ?? undefined,
        page: this.page(),
        pageSize: PAGE_SIZE
      })
      .subscribe({
        next: (result) => {
          this.products.set(result.items);
          this.totalPages.set(result.totalPages);
          this.totalCount.set(result.totalCount);
          this.isLoading.set(false);
        },
        error: () => {
          this.errorMessage.set('Failed to load products.');
          this.isLoading.set(false);
        }
      });
  }

  clearFilters(): void {
    this.colourFilter = '';
    this.minPriceFilter = null;
    this.maxPriceFilter = null;
    this.search();
  }

  openCreateModal(): void {
    this.showCreateModal.set(true);
  }

  closeCreateModal(): void {
    this.showCreateModal.set(false);
  }

  onProductCreated(): void {
    this.showCreateModal.set(false);
    this.search();
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
