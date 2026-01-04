import pandas as pd
import matplotlib.pyplot as plt
import os


EOA_POPULATION_SIZE = 100
MA_POPULATION_SIZE = 50

MA_LS_PROBABILITY = 0.2 
MA_LS_EPOCH_COUNT = 5
MA_IMPROVE_INIT = False

COST_LS = 1
COST_EOA = EOA_POPULATION_SIZE

MA_LS_CALLS = 1227
MA_EPOCHS = 25

NUMBER_OF_RUNS = 5
TOTAL_EPOCHS_ALL_RUNS = MA_EPOCHS * NUMBER_OF_RUNS

avg_ls_calls_per_epoch = MA_LS_CALLS / TOTAL_EPOCHS_ALL_RUNS

COST_MA = MA_POPULATION_SIZE + (avg_ls_calls_per_epoch * MA_LS_EPOCH_COUNT)

def plot_algorithm_stats(file_path, algorithm_name, line_color='red', fill_color='blue'):

    if not file_path or not os.path.exists(file_path):
        print(f"[{algorithm_name}] File was not found.")
        return

    print(f"[{algorithm_name}] Analyze : {file_path}")

    try:
        df = pd.read_csv(file_path, index_col=0)
    except Exception as e:
        print(f"Error: Can not read CSV: {e}")
        return

    df_transposed = df.T
    try:
        df_transposed.index = df_transposed.index.str.replace('Epoch_', '').astype(int)
    except AttributeError:
        pass

    mean_fitness = df_transposed.mean(axis=1)
    std_fitness = df_transposed.std(axis=1)

    last_epoch = df_transposed.index[-1]
    final_value = mean_fitness.iloc[-1]

    plt.figure(figsize=(12, 7))

    plt.fill_between(
        df_transposed.index,
        mean_fitness - std_fitness,
        mean_fitness + std_fitness,
        color=fill_color,
        alpha=0.2,
        label='Standard Deviation'
    )

    plt.plot(
        df_transposed.index,
        mean_fitness,
        color=line_color,
        linewidth=2,
        label = f'Average Fitness Value: {final_value:.1f}'
    )

    plt.scatter(
        last_epoch,
        final_value,
        color=line_color,
        s=70,
        zorder=5
    )

    plt.title(f'{algorithm_name} Analysis: {os.path.basename(file_path)}', fontsize=14)
    plt.xlabel('Epochs', fontsize=12)
    plt.ylabel('Fitness Value', fontsize=12)
    plt.grid(True, linestyle='--', alpha=0.5)
    plt.legend(loc='lower right')
    plt.tight_layout()

    base_name = os.path.splitext(file_path)[0]
    output_filename = base_name + ".png"

    plt.savefig(output_filename, dpi=300)
    print(f"[{algorithm_name}] Plot was saved successfully: {output_filename}")
    plt.show()



def plot_local_search(file_path):
    plot_algorithm_stats(file_path, "Local Search", line_color='red', fill_color='blue')


def plot_eoa(file_path):
    plot_algorithm_stats(file_path, "Evolutionary Algorithm", line_color='green', fill_color='lime')


def plot_memetic(file_path):
    plot_algorithm_stats(file_path, "Memetic Algorithm", line_color='purple', fill_color='magenta')


def process_and_plot(ax, file_path, algorithm_name, cost_per_epoch, start_cost=0, line_color='red', fill_color='blue',
                     max_ffe=None):
    if not file_path or not os.path.exists(file_path):
        print(f"[{algorithm_name}] File not found: {file_path}")
        return

    print(f"[{algorithm_name}] Processing...")

    try:
        df = pd.read_csv(file_path, index_col=0)
    except Exception as e:
        print(f"Error reading {file_path}: {e}")
        return

    df_transposed = df.T

    try:
        df_transposed.index = df_transposed.index.str.replace('Epoch_', '').astype(int)
    except AttributeError:
        pass

    ffe_index = (df_transposed.index * cost_per_epoch) + start_cost

    if max_ffe is not None:
        mask = ffe_index <= max_ffe
        if not mask.any(): mask[0] = True
        ffe_index = ffe_index[mask]
        df_transposed = df_transposed.loc[df_transposed.index[mask]]

    mean_fitness = df_transposed.mean(axis=1)
    std_fitness = df_transposed.std(axis=1)

    if len(ffe_index) == 0: return

    final_ffe = ffe_index[-1]
    final_val = mean_fitness.iloc[-1]

    ax.fill_between(
        ffe_index,
        mean_fitness - std_fitness,
        mean_fitness + std_fitness,
        color=fill_color,
        alpha=0.15,
        linewidth=0,
        label=None
    )

    ax.plot(
        ffe_index,
        mean_fitness,
        color=line_color,
        linewidth=2,
        label=f'{algorithm_name} (Final: {final_val:.1f})'
    )

    # Точка в конце
    ax.scatter(
        final_ffe,
        final_val,
        color=line_color,
        s=50,
        zorder=5,
        edgecolors='white'
    )


def plot_fintess_comparison(file_ls, file_eoa, file_ma, limit):
    plt.figure(figsize=(12, 7))
    ax = plt.gca()

    process_and_plot(ax, file_ls, "Local Search",
                     cost_per_epoch=COST_LS,
                     line_color='crimson', fill_color='red', max_ffe=limit)

    process_and_plot(ax, file_eoa, "Evolutionary Algorithm",
                     cost_per_epoch=COST_EOA,
                     line_color='forestgreen', fill_color='lime', max_ffe=limit)

    process_and_plot(ax, file_ma, "Memetic Algorithm",
                     cost_per_epoch=COST_MA,
                     # start_cost=START_COST_MA,
                     line_color='darkviolet', fill_color='magenta', max_ffe=limit)

    plt.title('Algorithm Comparison (Fitness Evaluations)', fontsize=16)
    plt.xlabel('Fitness Function Evaluations', fontsize=14)
    plt.ylabel('Fitness Value', fontsize=14)
    plt.grid(True, linestyle='--', alpha=0.5)
    plt.legend(loc='lower right', fontsize=12)
    plt.tight_layout()

    out_file = f"Comparison_{limit}.png"
    plt.savefig(out_file, dpi=300)
    print(f"Comparison plot saved: {out_file}")
    plt.show()


def main():
    ls_fit = "LocalSearch_Fitness.csv"

    eoa_fit = "EOA_Fitness.csv"

    ma_fit = "Memetic_Fitness.csv"

    limit = 2000

    print("--- Start Plotting ---")

    # LOCAL SEARCH
    if ls_fit:
        plot_local_search(ls_fit)
    else:
        print(f"File {ls_fit} was not found.")

    # EOA
    if eoa_fit:
        plot_eoa(eoa_fit)
    else:
        print(f"File {eoa_fit} was not found.")

    # MEMETIC
    if ma_fit:
        plot_memetic(ma_fit)
    else:
        print(f"File {ma_fit} was not found.")

    if ls_fit and eoa_fit and ma_fit:
        plot_fintess_comparison(ls_fit, eoa_fit, ma_fit, limit)
    else:
        print(f"One or all of those files: {ls_fit}, {eoa_fit}, {ma_fit} were not found.")

    print("--- Done ---")


if __name__ == "__main__":
    main()