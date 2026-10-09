import { HttpInterceptorFn, HttpResponse } from '@angular/common/http';
import { map } from 'rxjs';

export const envelopeInterceptor: HttpInterceptorFn = (request, next) =>
  next(request).pipe(
    map(response => response instanceof HttpResponse
      ? response.clone({ body: (response.body as { data?: unknown } | null)?.data ?? response.body })
      : response)
  );
