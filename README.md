# Blogplace.Backend

[![Code Quality](https://www.codefactor.io/repository/github/blogplace/blogplace.backend/badge)](https://www.codefactor.io/repository/github/blogplace/blogplace.backend)
[![Test Coverage](https://codecov.io/gh/Blogplace/Blogplace.Backend/graph/badge.svg?token=71BI6043KV)](https://codecov.io/gh/Blogplace/Blogplace.Backend)

## Zasady
#### Ogólne
- Lista zadań jest w [Issues](https://github.com/Blogplace/Blogplace.Backend/issues)
- Pracujemy na forkach repo, przy tworzeniu PR wybieramy repo bazowe
- Każde zadanie to osobny branch
  - Każdy branch musi zaczynać się od `\d+-.{3,}`, zaczyna się od ID issue, przykładowo `123-database-integration`
  - Nazwa brancha jest dowolna, ale powinna kojarzyć się z nazwą issue
- Każdy PR zawiera test, który udowadnia że wprowadzone zmiany działają i niczego nie psują
#### Unit Testy
- Zalecamy używać [fluentassertions](https://fluentassertions.com/), dzięki którym testy mają czytelniejszy opis w postaci `co gdzie i jak` zamiast `expected true but found false`.