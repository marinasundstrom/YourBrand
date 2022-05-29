#!sh

if [ -z "$CS" ]; then
    echo "Loading parameters..."

    CS="Server=localhost,1433;User Id=sa;Password=P@ssw0rd"

    echo "Connection String: ${CS}"
fi