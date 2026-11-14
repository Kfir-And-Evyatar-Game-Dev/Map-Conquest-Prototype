using UnityEngine;

/// <summary>Every few seconds pays each team for the regions linked to its base.</summary>
public class IncomeTicker : MonoBehaviour
{
    [SerializeField] private MatchSettings settings;
    [SerializeField] private SupplyNetwork network;
    [SerializeField] private ResourceBank[] banks;

    private Region[] regions;
    private float secondsUntilPayday;

    private void Awake()
    {
        regions = FindObjectsByType<Region>(FindObjectsSortMode.None);
        secondsUntilPayday = settings.IncomeIntervalSeconds;
    }

    private void Update()
    {
        secondsUntilPayday -= Time.deltaTime;
        if (secondsUntilPayday > 0f)
        {
            return;
        }

        secondsUntilPayday += settings.IncomeIntervalSeconds;
        foreach (ResourceBank bank in banks)
        {
            bank.Add(IncomeFor(bank.Team));
        }
    }

    public int IncomeFor(Team team)
    {
        int total = 0;
        foreach (Region region in regions)
        {
            if (region.Owner == team && network.IsConnected(region))
            {
                total += IncomeOf(region);
            }
        }

        return total;
    }

    private int IncomeOf(Region region)
    {
        if (region.IsBase)
        {
            return settings.BaseIncome;
        }

        return region.IsResource ? settings.ResourceRegionIncome : settings.RegionIncome;
    }
}